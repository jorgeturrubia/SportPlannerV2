import { Component, Input, signal, computed, ChangeDetectionStrategy, OnInit, OnDestroy, effect, ElementRef, ViewChild, ViewEncapsulation, TemplateRef, ContentChild } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-card-carousel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './card-carousel.component.html',
  styleUrls: ['./card-carousel.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None
})
export class CardCarouselComponent implements OnInit, OnDestroy {
  @ViewChild('carouselTrack', { read: ElementRef }) carouselTrack?: ElementRef;
  @ContentChild('cardTemplate', { read: TemplateRef }) cardTemplate?: TemplateRef<any>;

  @Input() set items(value: any[]) {
    this._items.set(value);
    this.createLoopedItems();
  }

  get items(): any[] {
    return this._items();
  }

  @Input() cardWidth: number = 320;
  @Input() gap: number = 24;
  @Input() autoPlayInterval: number = 3000;
  @Input() autoPlay: boolean = true;

  private _items = signal<any[]>([]);
  loopedItems = signal<any[]>([]);
  currentIndex = signal(0);
  isPaused = signal(false);
  isTransitioning = signal(true);

  private autoPlayTimer?: ReturnType<typeof setInterval>;
  private cloneCount = 3; // Número de clones antes y después

  // Computed signals
  totalCards = computed(() => this._items().length);
  totalLoopedCards = computed(() => this.loopedItems().length);

  // La tarjeta activa siempre es la del centro (tras los clones iniciales)
  centerIndex = computed(() => this.cloneCount);

  constructor() {
    effect(() => {
      if (this.autoPlay && !this.isPaused()) {
        this.startAutoPlay();
      } else {
        this.stopAutoPlay();
      }
    });
  }

  ngOnInit(): void {
    this.createLoopedItems();
    // Iniciar en el primer item real (después de los clones)
    this.normalizeCloneCount();
    this.currentIndex.set(this.cloneCount);

    console.log('Carousel initialized:');
    console.log('- Clone count:', this.cloneCount);
    console.log('- Current index:', this.currentIndex());
    console.log('- Total items:', this.totalCards());
    console.log('- Looped items:', this.totalLoopedCards());

    if (this.autoPlay) {
      this.startAutoPlay();
    }
  }

  ngOnDestroy(): void {
    this.stopAutoPlay();
  }

  private createLoopedItems(): void {
    const items = this._items();
    if (items.length === 0) return;

    // Ajustar cloneCount si la cantidad de items es menor
    this.normalizeCloneCount();

    // Crear array con clones: [últimos 3] + [originales] + [primeros 3]
    const looped = [
      ...items.slice(-this.cloneCount), // Últimos elementos
      ...items,                          // Elementos originales
      ...items.slice(0, this.cloneCount) // Primeros elementos
    ];

    this.loopedItems.set(looped);
  }

  private startAutoPlay(): void {
    this.stopAutoPlay();
    this.autoPlayTimer = setInterval(() => {
      this.autoAdvance();
    }, this.autoPlayInterval);
  }

  private stopAutoPlay(): void {
    if (this.autoPlayTimer) {
      clearInterval(this.autoPlayTimer);
      this.autoPlayTimer = undefined;
    }
  }

  private autoAdvance(): void {
    if (this.isPaused()) return;
    this.next();
  }

  getTransform(): string {
    // Centrar la tarjeta activa en el viewport
    const vwCenter = (typeof window !== 'undefined' ? window.innerWidth / 2 : 800);
    const centerOffset = vwCenter - (this.cardWidth / 2);

    // La posicion en pixeles del elemento en el track (considerando gap)
    const trackOffset = this.currentIndex() * (this.cardWidth + this.gap);

    // El offset final: desplazar el track para que el elemento esté centrado
    const offset = trackOffset - centerOffset;

    return `translateX(-${offset}px)`;
  }

  prev(): void {
    this.isPaused.set(true);
    this.isTransitioning.set(true);
    this.currentIndex.update(i => i - 1);

    this.checkLoopPosition();

    setTimeout(() => this.isPaused.set(false), 5000);
  }

  next(): void {
    this.isPaused.set(true);
    this.isTransitioning.set(true);
    this.currentIndex.update(i => i + 1);

    this.checkLoopPosition();

    setTimeout(() => this.isPaused.set(false), 5000);
  }

  private checkLoopPosition(): void {
    // Después de la transición, verificar si necesitamos reposicionar
    setTimeout(() => {
      const current = this.currentIndex();
      const totalOriginal = this.totalCards();

      // Si estamos en los clones del final, saltar al inicio real
      if (current >= this.cloneCount + totalOriginal) {
        this.isTransitioning.set(false);
        this.currentIndex.set(this.cloneCount + (current - this.cloneCount - totalOriginal));

        // Restaurar transición en el siguiente frame
        setTimeout(() => this.isTransitioning.set(true), 50);
      }

      // Si estamos en los clones del inicio, saltar al final real
      if (current < this.cloneCount) {
        this.isTransitioning.set(false);
        this.currentIndex.set(this.cloneCount + totalOriginal + current - this.cloneCount);

        setTimeout(() => this.isTransitioning.set(true), 50);
      }
    }, 600); // Duración de la transición CSS
  }

  goToIndex(realIndex: number): void {
    this.isPaused.set(true);
    this.isTransitioning.set(true);
    // Convertir índice real a índice en el array looped
    this.currentIndex.set(this.cloneCount + realIndex);
    setTimeout(() => this.isPaused.set(false), 5000);
  }

  trackByIndex(_idx: number, _item: any) {
    return _idx;
  }

  // Ajusta cloneCount si hay pocos items para evitar clones mayores que el array
  private normalizeCloneCount() {
    const total = this._items().length;
    if (total === 0) return;
    if (this.cloneCount >= total) {
      // Mantener al menos 1 clone, pero no más que la mitad del array
      this.cloneCount = Math.max(1, Math.floor(total / 2));
    }
  }

  getDotIndices(): number[] {
    return Array.from({ length: this.totalCards() }, (_, i) => i);
  }

  getCurrentDotIndex(): number {
    const current = this.currentIndex();
    const totalOriginal = this.totalCards();

    // Convertir índice looped a índice real
    if (current < this.cloneCount) {
      return totalOriginal + (current - this.cloneCount);
    } else if (current >= this.cloneCount + totalOriginal) {
      return current - this.cloneCount - totalOriginal;
    } else {
      return current - this.cloneCount;
    }
  }

  onMouseEnter(): void {
    this.isPaused.set(true);
  }

  onMouseLeave(): void {
    this.isPaused.set(false);
  }

  isCardActive(loopedIndex: number): boolean {
    // La tarjeta activa es siempre la que está en currentIndex
    const isActive = loopedIndex === this.currentIndex();
    if (loopedIndex <= 4) { // Solo log para las primeras tarjetas para no saturar
      console.log(`Card ${loopedIndex} active check: ${isActive} (currentIndex: ${this.currentIndex()})`);
    }
    return isActive;
  }

  // Distancia absoluta entre el índice actual y uno pasado
  absIndexDistance(idx: number): number {
    return Math.abs(this.currentIndex() - idx);
  }

  // Traer una tarjeta al frente (ajustando el índice actual)
  bringToFront(loopedIndex: number): void {
    // Si ya está delante, no hacer nada
    if (this.currentIndex() === loopedIndex) return;

    this.isTransitioning.set(true);
    this.currentIndex.set(loopedIndex);

    // Revisar y corregir la posición si entramos en clones
    this.checkLoopPosition();
  }
}