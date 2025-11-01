/**
 * Sport Exercise Animation System
 * Sistema profesional para crear y reproducir ejercicios deportivos animados
 */

import { CourtType, CourtRenderOptions } from './court-templates.model';

export interface Point {
  x: number; // 0-100 (porcentaje del ancho de la cancha)
  y: number; // 0-100 (porcentaje del alto de la cancha)
}

export interface AnimatedElement {
  id: string;
  type: 'player' | 'ball' | 'cone' | 'marker' | 'target';
  position: Point;
  rotation?: number; // grados
  scale?: number;
  opacity?: number;
  label?: string;
  color?: string;
}

export interface Action {
  type: 'pass' | 'dribble' | 'shoot' | 'move' | 'receive' | 'defense';
  playerId?: string;
  target?: Point;
  duration?: number; // ms
  description?: string;
}

export interface Frame {
  timestamp: number; // milisegundos desde el inicio
  elements: AnimatedElement[];
  actions?: Action[];
  annotations?: {
    text: string;
    position: Point;
    duration?: number;
  }[];
}

export interface ExerciseAnimation {
  id: string;
  name: string;
  description: string;
  sport: CourtType;
  court: CourtConfig;
  duration: number; // milisegundos totales
  frames: Frame[];
  metadata?: {
    difficulty?: 'beginner' | 'intermediate' | 'advanced';
    focus?: string[];
    playerCount?: number;
  };
}

export interface CourtConfig {
  type: CourtType;
  renderOptions?: CourtRenderOptions;
  backgroundColor?: string; // Override del color por defecto
}

export interface PlaybackState {
  isPlaying: boolean;
  currentTime: number;
  speed: number; // 0.5x, 1x, 2x
  loop: boolean;
}

// Helper para crear animaciones fácilmente
export class AnimationBuilder {
  private frames: Frame[] = [];
  private currentTimestamp = 0;

  addFrame(duration: number, configurator: (frame: Frame) => void): this {
    const frame: Frame = {
      timestamp: this.currentTimestamp,
      elements: [],
      actions: [],
      annotations: []
    };
    
    configurator(frame);
    this.frames.push(frame);
    this.currentTimestamp += duration;
    
    return this;
  }

  addPlayer(frame: Frame, id: string, x: number, y: number, label?: string, color?: string): AnimatedElement {
    const player: AnimatedElement = {
      id,
      type: 'player',
      position: { x, y },
      label,
      color: color || '#3B82F6',
      scale: 1,
      opacity: 1
    };
    frame.elements.push(player);
    return player;
  }

  addBall(frame: Frame, x: number, y: number): AnimatedElement {
    const ball: AnimatedElement = {
      id: 'ball',
      type: 'ball',
      position: { x, y },
      scale: 1,
      opacity: 1
    };
    frame.elements.push(ball);
    return ball;
  }

  addCone(frame: Frame, id: string, x: number, y: number, color?: string): AnimatedElement {
    const cone: AnimatedElement = {
      id,
      type: 'cone',
      position: { x, y },
      color: color || '#F59E0B',
      scale: 1,
      opacity: 1
    };
    frame.elements.push(cone);
    return cone;
  }

  addAction(frame: Frame, action: Action): void {
    if (!frame.actions) frame.actions = [];
    frame.actions.push(action);
  }

  addAnnotation(frame: Frame, text: string, x: number, y: number, duration = 2000): void {
    if (!frame.annotations) frame.annotations = [];
    frame.annotations.push({
      text,
      position: { x, y },
      duration
    });
  }

  build(config: Omit<ExerciseAnimation, 'frames' | 'duration'>): ExerciseAnimation {
    return {
      ...config,
      frames: this.frames,
      duration: this.currentTimestamp
    };
  }
}
