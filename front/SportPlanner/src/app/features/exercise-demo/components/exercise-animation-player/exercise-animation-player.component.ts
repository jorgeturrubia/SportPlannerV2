import { Component, input, signal, computed, effect, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ExerciseAnimation, Frame, AnimatedElement, Point, PlaybackState } from '../../models/exercise-animation.model';
import { getCourtTemplate, COURT_DIMENSIONS } from '../../models/court-templates.model';

@Component({
  selector: 'app-exercise-animation-player',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './exercise-animation-player.component.html',
  styleUrls: ['./exercise-animation-player.component.css']
})
export class ExerciseAnimationPlayerComponent implements OnDestroy {
  // Inputs
  exercise = input.required<ExerciseAnimation>();
  autoPlay = input<boolean>(false);

  // State
  playbackState = signal<PlaybackState>({
    isPlaying: false,
    currentTime: 0,
    speed: 1,
    loop: true
  });

  currentFrame = signal<number>(0);
  interpolatedElements = signal<AnimatedElement[]>([]);
  activeAnnotations = signal<Array<{ text: string; position: Point; opacity: number }>>([]);

  // Court rendering computed properties
  courtTemplate = computed(() => {
    const ex = this.exercise();
    if (!ex || !ex.court || !ex.court.type) {
      return this.sanitizer.bypassSecurityTrustHtml('');
    }
    
    // Normalizar tipo de cancha (basketball-full -> basketball)
    let courtType = ex.court.type;
    if (courtType.toString().includes('basketball')) {
      courtType = 'basketball' as any;
    }
    
    const template = getCourtTemplate(courtType);
    const renderOptions = ex.court.renderOptions || {
      showLines: true,
      showThreePointLine: true,
      showKeyArea: true,
      showCenterCircle: true
    };
    const svgContent = template.renderSVG(renderOptions);
    return this.sanitizer.bypassSecurityTrustHtml(svgContent);
  });

  courtViewBox = computed(() => {
    const ex = this.exercise();
    if (!ex || !ex.court || !ex.court.type) {
      return '0 0 186.7 100'; // Default viewBox
    }
    
    // Para baloncesto (normalizar variantes como basketball-full)
    const courtType = ex.court.type.toString();
    if (courtType.includes('basketball')) {
      return '0 0 186.7 100';
    }
    
    // Para otros deportes, validar que existe en COURT_DIMENSIONS
    const dimensions = COURT_DIMENSIONS[ex.court.type];
    if (!dimensions) {
      console.warn(`⚠️ Tipo de cancha no encontrado: ${ex.court.type}, usando dimensiones por defecto`);
      return '0 0 186.7 100';
    }
    
    // Usar dimensiones proporcionales
    return `0 0 ${dimensions.width * 6.67} ${dimensions.height * 6.67}`;
  });

  courtBackgroundColor = computed(() => {
    const ex = this.exercise();
    if (!ex || !ex.court) {
      return '#e6f2e6'; // Default background
    }
    if (ex.court.backgroundColor) {
      return ex.court.backgroundColor;
    }
    const template = getCourtTemplate(ex.court.type);
    return template.backgroundColor;
  });

  // Computed
  progress = computed(() => {
    const state = this.playbackState();
    const ex = this.exercise();
    return (state.currentTime / ex.duration) * 100;
  });

  formattedTime = computed(() => {
    const time = this.playbackState().currentTime;
    const minutes = Math.floor(time / 60000);
    const seconds = Math.floor((time % 60000) / 1000);
    const ms = Math.floor((time % 1000) / 10);
    return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}.${ms.toString().padStart(2, '0')}`;
  });

  totalDuration = computed(() => {
    const duration = this.exercise().duration;
    const minutes = Math.floor(duration / 60000);
    const seconds = Math.floor((duration % 60000) / 1000);
    return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
  });

  // Private
  private animationFrameId: number | null = null;
  private lastTimestamp = 0;

  constructor(private sanitizer: DomSanitizer) {
    effect(() => {
      if (this.autoPlay() && this.exercise()) {
        this.play();
      }
    });
  }

  ngOnDestroy(): void {
    this.stop();
  }

  play(): void {
    if (this.playbackState().isPlaying) return;

    // Validar que hay animación cargada
    const ex = this.exercise();
    if (!ex || !ex.frames || ex.frames.length === 0) {
      console.error('❌ No se puede reproducir: no hay animación cargada');
      return;
    }

    this.playbackState.update(state => ({ ...state, isPlaying: true }));
    this.lastTimestamp = performance.now();
    this.animate();
  }

  pause(): void {
    console.log('⏸️ pause() llamado - animationFrameId:', this.animationFrameId);
    
    // Primero cancelar el frame de animación
    if (this.animationFrameId !== null) {
      cancelAnimationFrame(this.animationFrameId);
      console.log('✅ cancelAnimationFrame ejecutado');
      this.animationFrameId = null;
    }
    
    // Luego actualizar el estado
    this.playbackState.update(state => ({ ...state, isPlaying: false }));
    console.log('✅ Estado actualizado a isPlaying: false');
  }

  stop(): void {
    this.pause();
    this.playbackState.update(state => ({ ...state, currentTime: 0 }));
    this.currentFrame.set(0);
    this.updateInterpolatedElements(0);
  }

  restart(): void {
    this.stop();
    setTimeout(() => this.play(), 100);
  }

  togglePlayPause(): void {
    const currentState = this.playbackState().isPlaying;
    console.log('🎮 togglePlayPause - Estado actual:', currentState);
    
    if (currentState) {
      console.log('⏸️ Pausando...');
      this.pause();
    } else {
      console.log('▶️ Reproduciendo...');
      this.play();
    }
    
    console.log('🎮 togglePlayPause - Nuevo estado:', this.playbackState().isPlaying);
  }

  setSpeed(speed: number): void {
    this.playbackState.update(state => ({ ...state, speed }));
  }

  toggleLoop(): void {
    this.playbackState.update(state => ({ ...state, loop: !state.loop }));
  }

  seekTo(time: number): void {
    this.playbackState.update(state => ({ ...state, currentTime: time }));
    this.updateInterpolatedElements(time);
  }

  private animate = (): void => {
    const isPlaying = this.playbackState().isPlaying;
    console.log('🎬 animate() - isPlaying:', isPlaying);
    
    if (!isPlaying) {
      console.log('⏹️ Animación detenida (isPlaying = false)');
      return;
    }

    const now = performance.now();
    const deltaTime = (now - this.lastTimestamp) * this.playbackState().speed;
    this.lastTimestamp = now;

    const newTime = this.playbackState().currentTime + deltaTime;
    const duration = this.exercise().duration;

    if (newTime >= duration) {
      if (this.playbackState().loop) {
        this.playbackState.update(state => ({ ...state, currentTime: 0 }));
        this.updateInterpolatedElements(0);
      } else {
        this.stop();
        return;
      }
    } else {
      this.playbackState.update(state => ({ ...state, currentTime: newTime }));
      this.updateInterpolatedElements(newTime);
    }

    // Solo programar el siguiente frame si todavía estamos reproduciendo
    const stillPlaying = this.playbackState().isPlaying;
    console.log('🔄 Verificando antes de programar siguiente frame - stillPlaying:', stillPlaying);
    
    if (stillPlaying) {
      this.animationFrameId = requestAnimationFrame(this.animate);
      console.log('➡️ Siguiente frame programado - ID:', this.animationFrameId);
    } else {
      console.log('⏹️ NO se programa siguiente frame (ya no está playing)');
    }
  };

  private updateInterpolatedElements(time: number): void {
    const ex = this.exercise();
    if (!ex || !ex.frames || ex.frames.length === 0) {
      console.error('❌ updateInterpolatedElements: no hay frames disponibles');
      return;
    }

    const frames = ex.frames;
    
    // Find current and next frame
    let currentFrameIndex = 0;
    for (let i = 0; i < frames.length - 1; i++) {
      if (time >= frames[i].timestamp && time < frames[i + 1].timestamp) {
        currentFrameIndex = i;
        break;
      }
    }

    if (time >= frames[frames.length - 1].timestamp) {
      currentFrameIndex = frames.length - 1;
    }

    this.currentFrame.set(currentFrameIndex);

    const currentFrame = frames[currentFrameIndex];
    const nextFrame = frames[currentFrameIndex + 1];

    if (!nextFrame) {
      // Last frame, no interpolation
      this.interpolatedElements.set(currentFrame.elements);
      this.updateAnnotations(currentFrame, time);
      return;
    }

    // Calculate interpolation factor (0 to 1)
    const frameStartTime = currentFrame.timestamp;
    const frameEndTime = nextFrame.timestamp;
    const frameDuration = frameEndTime - frameStartTime;
    const t = Math.min(1, (time - frameStartTime) / frameDuration);

    // Easing function (easeInOutCubic for smooth motion)
    const easedT = t < 0.5
      ? 4 * t * t * t
      : 1 - Math.pow(-2 * t + 2, 3) / 2;

    // Interpolate elements
    const interpolated = currentFrame.elements.map(currentEl => {
      const nextEl = nextFrame.elements.find(e => e.id === currentEl.id);

      if (!nextEl) {
        // Element doesn't exist in next frame, fade out
        return {
          ...currentEl,
          opacity: currentEl.opacity! * (1 - easedT)
        };
      }

      // Interpolate position
      const x = this.lerp(currentEl.position.x, nextEl.position.x, easedT);
      const y = this.lerp(currentEl.position.y, nextEl.position.y, easedT);

      // Interpolate other properties
      const rotation = this.lerp(currentEl.rotation || 0, nextEl.rotation || 0, easedT);
      const scale = this.lerp(currentEl.scale || 1, nextEl.scale || 1, easedT);
      const opacity = this.lerp(currentEl.opacity || 1, nextEl.opacity || 1, easedT);

      return {
        ...currentEl,
        position: { x, y },
        rotation,
        scale,
        opacity
      };
    });

    // Add new elements from next frame that don't exist in current (fade in)
    nextFrame.elements.forEach(nextEl => {
      if (!currentFrame.elements.find(e => e.id === nextEl.id)) {
        interpolated.push({
          ...nextEl,
          opacity: (nextEl.opacity || 1) * easedT
        });
      }
    });

    this.interpolatedElements.set(interpolated);
    this.updateAnnotations(currentFrame, time);
  }

  private updateAnnotations(frame: Frame, time: number): void {
    if (!frame.annotations) {
      this.activeAnnotations.set([]);
      return;
    }

    const active = frame.annotations
      .map(annotation => {
        const timeSinceFrameStart = time - frame.timestamp;
        const duration = annotation.duration || 2000;
        
        if (timeSinceFrameStart < 0 || timeSinceFrameStart > duration) {
          return null;
        }

        // Fade in/out
        let opacity = 1;
        const fadeTime = 300;
        
        if (timeSinceFrameStart < fadeTime) {
          opacity = timeSinceFrameStart / fadeTime;
        } else if (timeSinceFrameStart > duration - fadeTime) {
          opacity = (duration - timeSinceFrameStart) / fadeTime;
        }

        return {
          text: annotation.text,
          position: annotation.position,
          opacity
        };
      })
      .filter(a => a !== null) as Array<{ text: string; position: Point; opacity: number }>;

    this.activeAnnotations.set(active);
  }

  private lerp(start: number, end: number, t: number): number {
    return start + (end - start) * t;
  }

  // Helper for court coordinates (convert 0-100 to actual SVG coordinates)
  getCourtX(x: number): number {
    const ex = this.exercise();
    if (!ex || !ex.court || !ex.court.type) {
      return (x / 100) * 186.7; // Default para basketball
    }
    
    const courtType = ex.court.type.toString();
    if (courtType.includes('basketball')) {
      // Basketball: viewBox es 186.7 x 100
      // x va de 0-100 en el ejercicio, mapear a 0-186.7
      return (x / 100) * 186.7;
    }
    
    // Para otros deportes, ajustar según dimensiones
    const dimensions = COURT_DIMENSIONS[ex.court.type];
    if (!dimensions) {
      return (x / 100) * 186.7; // Fallback
    }
    return (x / 100) * dimensions.width * 6.67;
  }

  getCourtY(y: number): number {
    const ex = this.exercise();
    if (!ex || !ex.court || !ex.court.type) {
      return y; // Default para basketball
    }
    
    const courtType = ex.court.type.toString();
    if (courtType.includes('basketball')) {
      // Basketball: viewBox es 186.7 x 100
      // y va de 0-100 en el ejercicio, mapear a 0-100 (coincide)
      return y;
    }
    
    // Para otros deportes, ajustar según dimensiones
    const dimensions = COURT_DIMENSIONS[ex.court.type];
    if (!dimensions) {
      return y; // Fallback
    }
    return (y / 100) * dimensions.height * 6.67;
  }

  // Timeline click handler
  onTimelineClick(event: MouseEvent): void {
    const timeline = event.currentTarget as HTMLElement;
    const rect = timeline.getBoundingClientRect();
    const clickX = event.clientX - rect.left;
    const percentage = clickX / rect.width;
    const newTime = percentage * this.exercise().duration;
    this.seekTo(Math.max(0, Math.min(newTime, this.exercise().duration)));
  }
}
