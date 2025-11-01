/**
 * Court Templates - Plantillas profesionales de canchas deportivas
 * Proporciona SVG templates realistas para diferentes deportes
 */

export type CourtType = 'basketball' | 'football' | 'handball' | 'volleyball' | 'futsal';

export interface CourtDimensions {
  width: number;  // metros reales
  height: number; // metros reales
  aspectRatio: string; // para CSS
}

export interface CourtTemplate {
  type: CourtType;
  name: string;
  dimensions: CourtDimensions;
  backgroundColor: string;
  renderSVG: (options?: CourtRenderOptions) => string;
}

export interface CourtRenderOptions {
  showLines?: boolean;
  showThreePointLine?: boolean;
  showKeyArea?: boolean;
  showCenterCircle?: boolean;
  showGoals?: boolean;
  lineColor?: string;
  lineWidth?: number;
  opacity?: number;
}

/**
 * Dimensiones oficiales de canchas deportivas
 */
export const COURT_DIMENSIONS: Record<CourtType, CourtDimensions> = {
  basketball: {
    width: 28,
    height: 15,
    aspectRatio: '28 / 15' // 1.867:1
  },
  football: {
    width: 105,
    height: 68,
    aspectRatio: '105 / 68' // 1.544:1
  },
  handball: {
    width: 40,
    height: 20,
    aspectRatio: '40 / 20' // 2:1
  },
  volleyball: {
    width: 18,
    height: 9,
    aspectRatio: '18 / 9' // 2:1
  },
  futsal: {
    width: 40,
    height: 20,
    aspectRatio: '40 / 20' // 2:1
  }
};

/**
 * Genera SVG de una pista de baloncesto profesional (FIBA)
 * Incluye: perímetro, línea central, círculo central, zonas restringidas,
 * líneas de 3 puntos, líneas de tiros libres, canastas
 */
function renderBasketballCourt(options: CourtRenderOptions = {}): string {
  const {
    showLines = true,
    showThreePointLine = true,
    showKeyArea = true,
    showCenterCircle = true,
    lineColor = '#FFFFFF',
    lineWidth = 0.2,
    opacity = 1
  } = options;

  // Usando coordenadas proporcionales (0-100 en X, 0-100 en Y basado en aspect ratio)
  // La cancha real es 28m x 15m (ratio 1.867:1)
  // Para mantener proporciones: Y va de 0-100, X se ajusta proporcionalmente
  
  const viewBoxWidth = 186.7; // 28m * 6.67 para escalar
  const viewBoxHeight = 100;  // 15m * 6.67 para escalar
  
  // Dimensiones clave (en unidades SVG)
  const keyWidth = 32.67;      // Ancho de la zona (4.9m)
  const keyHeight = 38.67;     // Alto de la zona (5.8m)
  const freeThrowLineX = 24;   // Línea de tiro libre (3.6m)
  const threePointRadius = 42; // Radio línea de 3 puntos (6.75m)
  const centerCircleRadius = 12; // Radio círculo central (1.8m)
  const restrictedCircleRadius = 8.33; // Radio semicírculo zona (1.25m)

  return `
    <g class="basketball-court-template" opacity="${opacity}">
      ${showLines ? `
        <!-- Perímetro de la cancha -->
        <rect 
          x="2" y="2" 
          width="${viewBoxWidth - 4}" 
          height="${viewBoxHeight - 4}" 
          fill="none" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth * 1.5}" 
          stroke-linecap="square" 
          stroke-linejoin="miter"
        />

        <!-- Línea central -->
        <line 
          x1="${viewBoxWidth / 2}" y1="2" 
          x2="${viewBoxWidth / 2}" y2="${viewBoxHeight - 2}" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}" 
          stroke-linecap="square"
        />

        <!-- Pequeñas marcas en la línea central (arriba y abajo) -->
        <line x1="${viewBoxWidth / 2 - 2}" y1="2" x2="${viewBoxWidth / 2 + 2}" y2="2" 
              stroke="${lineColor}" stroke-width="${lineWidth * 1.2}"/>
        <line x1="${viewBoxWidth / 2 - 2}" y1="${viewBoxHeight - 2}" x2="${viewBoxWidth / 2 + 2}" y2="${viewBoxHeight - 2}" 
              stroke="${lineColor}" stroke-width="${lineWidth * 1.2}"/>
      ` : ''}

      ${showCenterCircle ? `
        <!-- Círculo central (radio 1.8m) -->
        <circle 
          cx="${viewBoxWidth / 2}" 
          cy="${viewBoxHeight / 2}" 
          r="${centerCircleRadius}" 
          fill="none" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
        />
        
        <!-- Punto central -->
        <circle 
          cx="${viewBoxWidth / 2}" 
          cy="${viewBoxHeight / 2}" 
          r="0.8" 
          fill="${lineColor}"
        />
      ` : ''}

      ${showKeyArea ? `
        <!-- LADO IZQUIERDO -->
        <!-- Zona restringida izquierda -->
        <rect 
          x="2" y="${(viewBoxHeight - keyHeight) / 2}" 
          width="${keyWidth}" height="${keyHeight}" 
          fill="none" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
          stroke-linejoin="miter"
        />
        
        <!-- Semicírculo punteado zona izquierda -->
        <path 
          d="M ${2 + keyWidth} ${viewBoxHeight / 2 - restrictedCircleRadius} 
             A ${restrictedCircleRadius} ${restrictedCircleRadius} 0 0 1 ${2 + keyWidth} ${viewBoxHeight / 2 + restrictedCircleRadius}" 
          fill="none" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
          stroke-dasharray="1.5,1.5"
        />
        
        <!-- Círculo pequeño inferior izquierdo (restricción) -->
        <circle 
          cx="${2 + keyWidth}" 
          cy="${viewBoxHeight / 2}" 
          r="0.8" 
          fill="${lineColor}"
        />

        <!-- Línea de tiro libre izquierda (punteada) -->
        <line 
          x1="${freeThrowLineX}" y1="${(viewBoxHeight - keyHeight) / 2}" 
          x2="${freeThrowLineX}" y2="${(viewBoxHeight + keyHeight) / 2}" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}" 
          stroke-dasharray="1.5,1.5"
        />

        <!-- Marcas de posición izquierda (8 marcas) -->
        ${generateReboundMarks(2, keyHeight, viewBoxHeight, lineColor, lineWidth, 'left')}

        <!-- LADO DERECHO -->
        <!-- Zona restringida derecha -->
        <rect 
          x="${viewBoxWidth - 2 - keyWidth}" 
          y="${(viewBoxHeight - keyHeight) / 2}" 
          width="${keyWidth}" 
          height="${keyHeight}" 
          fill="none" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
          stroke-linejoin="miter"
        />
        
        <!-- Semicírculo punteado zona derecha -->
        <path 
          d="M ${viewBoxWidth - 2 - keyWidth} ${viewBoxHeight / 2 - restrictedCircleRadius} 
             A ${restrictedCircleRadius} ${restrictedCircleRadius} 0 0 0 ${viewBoxWidth - 2 - keyWidth} ${viewBoxHeight / 2 + restrictedCircleRadius}" 
          fill="none" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
          stroke-dasharray="1.5,1.5"
        />
        
        <!-- Círculo pequeño inferior derecho (restricción) -->
        <circle 
          cx="${viewBoxWidth - 2 - keyWidth}" 
          cy="${viewBoxHeight / 2}" 
          r="0.8" 
          fill="${lineColor}"
        />

        <!-- Línea de tiro libre derecha (punteada) -->
        <line 
          x1="${viewBoxWidth - freeThrowLineX}" y1="${(viewBoxHeight - keyHeight) / 2}" 
          x2="${viewBoxWidth - freeThrowLineX}" y2="${(viewBoxHeight + keyHeight) / 2}" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}" 
          stroke-dasharray="1.5,1.5"
        />

        <!-- Marcas de posición derecha -->
        ${generateReboundMarks(viewBoxWidth - 2, keyHeight, viewBoxHeight, lineColor, lineWidth, 'right')}
      ` : ''}

      ${showThreePointLine ? `
        <!-- LÍNEA DE 3 PUNTOS IZQUIERDA -->
        <!-- Parte recta superior -->
        <line 
          x1="6.8" y1="2" 
          x2="6.8" y2="${viewBoxHeight / 2 - 29}" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
          stroke-linecap="square"
        />
        <!-- Arco -->
        <path 
          d="M 6.8 ${viewBoxHeight / 2 - 29} 
             A ${threePointRadius} ${threePointRadius} 0 0 1 6.8 ${viewBoxHeight / 2 + 29}" 
          fill="none" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
        />
        <!-- Parte recta inferior -->
        <line 
          x1="6.8" y1="${viewBoxHeight / 2 + 29}" 
          x2="6.8" y2="${viewBoxHeight - 2}" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
          stroke-linecap="square"
        />

        <!-- LÍNEA DE 3 PUNTOS DERECHA -->
        <!-- Parte recta superior -->
        <line 
          x1="${viewBoxWidth - 6.8}" y1="2" 
          x2="${viewBoxWidth - 6.8}" y2="${viewBoxHeight / 2 - 29}" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
          stroke-linecap="square"
        />
        <!-- Arco -->
        <path 
          d="M ${viewBoxWidth - 6.8} ${viewBoxHeight / 2 - 29} 
             A ${threePointRadius} ${threePointRadius} 0 0 0 ${viewBoxWidth - 6.8} ${viewBoxHeight / 2 + 29}" 
          fill="none" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
        />
        <!-- Parte recta inferior -->
        <line 
          x1="${viewBoxWidth - 6.8}" y1="${viewBoxHeight / 2 + 29}" 
          x2="${viewBoxWidth - 6.8}" y2="${viewBoxHeight - 2}" 
          stroke="${lineColor}" 
          stroke-width="${lineWidth}"
          stroke-linecap="square"
        />
      ` : ''}

      <!-- Canastas y aros -->
      ${renderBasketHoop(2, viewBoxHeight / 2, lineColor, 'left')}
      ${renderBasketHoop(viewBoxWidth - 2, viewBoxHeight / 2, lineColor, 'right')}

      <!-- Pequeñas marcas en los laterales (para orientación) -->
      ${generateSideMarks(viewBoxWidth, viewBoxHeight, lineColor, lineWidth)}
    </g>
  `;
}

/**
 * Genera marcas de posición para rebotes (8 marcas por lado)
 */
function generateReboundMarks(
  xBase: number, 
  keyHeight: number, 
  courtHeight: number, 
  color: string, 
  lineWidth: number,
  side: 'left' | 'right'
): string {
  const marks = [];
  const startY = (courtHeight - keyHeight) / 2;
  const spacing = keyHeight / 8;
  const markLength = 1.2;
  const markWidth = lineWidth * 1.2;
  
  for (let i = 1; i <= 7; i++) {
    // Skip posición central (i === 4)
    if (i === 4) continue;
    
    const y = startY + spacing * i;
    const xStart = side === 'left' ? xBase + keyHeight/2 - 0.2 : xBase - keyHeight/2 - markLength + 0.2;
    
    marks.push(`
      <line 
        x1="${xStart}" 
        y1="${y}" 
        x2="${xStart + markLength}" 
        y2="${y}" 
        stroke="${color}"
        stroke-width="${markWidth}"
        stroke-linecap="square"
      />
    `);
  }
  
  return marks.join('');
}

/**
 * Renderiza un aro de baloncesto con tablero
 */
function renderBasketHoop(x: number, y: number, color: string, side: 'left' | 'right'): string {
  const hoopOffset = side === 'left' ? 4 : -4;
  const hoopX = x + hoopOffset;
  
  return `
    <g class="basket-hoop">
      <!-- Aro pequeño (representación simplificada) -->
      <circle 
        cx="${hoopX}" 
        cy="${y}" 
        r="1.2" 
        fill="none" 
        stroke="${color}" 
        stroke-width="0.25"
      />
      
      <!-- Punto de anclaje del aro -->
      <circle 
        cx="${hoopX}" 
        cy="${y}" 
        r="0.4" 
        fill="${color}"
        opacity="0.7"
      />
    </g>
  `;
}

/**
 * Genera pequeñas marcas en los laterales de la cancha
 */
function generateSideMarks(width: number, height: number, color: string, lineWidth: number): string {
  const markLength = 1.5;
  const marks: string[] = [];
  
  // Marcas laterales cada cierta distancia
  const positions = [20, 40, 60, 80]; // Posiciones en porcentaje del alto
  
  positions.forEach(pct => {
    const y = (height * pct) / 100;
    
    // Lado izquierdo
    marks.push(`
      <line x1="2" y1="${y}" x2="${2 + markLength}" y2="${y}" 
            stroke="${color}" stroke-width="${lineWidth}" stroke-linecap="square"/>
    `);
    
    // Lado derecho
    marks.push(`
      <line x1="${width - 2}" y1="${y}" x2="${width - 2 - markLength}" y2="${y}" 
            stroke="${color}" stroke-width="${lineWidth}" stroke-linecap="square"/>
    `);
  });
  
  return marks.join('');
}

/**
 * Genera SVG de una cancha de fútbol (placeholder para futura implementación)
 */
function renderFootballField(options: CourtRenderOptions = {}): string {
  return `
    <g class="football-field-template">
      <text x="50%" y="50%" text-anchor="middle" fill="#666" font-size="8">
        Cancha de Fútbol - Próximamente
      </text>
    </g>
  `;
}

/**
 * Genera SVG de una cancha de balonmano (placeholder para futura implementación)
 */
function renderHandballCourt(options: CourtRenderOptions = {}): string {
  return `
    <g class="handball-court-template">
      <text x="50%" y="50%" text-anchor="middle" fill="#666" font-size="8">
        Cancha de Balonmano - Próximamente
      </text>
    </g>
  `;
}

/**
 * Registro de plantillas de canchas disponibles
 */
export const COURT_TEMPLATES: Record<CourtType, CourtTemplate> = {
  basketball: {
    type: 'basketball',
    name: 'Cancha de Baloncesto (FIBA)',
    dimensions: COURT_DIMENSIONS.basketball,
    backgroundColor: '#C17B3A', // Parquet color
    renderSVG: renderBasketballCourt
  },
  football: {
    type: 'football',
    name: 'Campo de Fútbol',
    dimensions: COURT_DIMENSIONS.football,
    backgroundColor: '#4A7C44', // Verde césped
    renderSVG: renderFootballField
  },
  handball: {
    type: 'handball',
    name: 'Cancha de Balonmano',
    dimensions: COURT_DIMENSIONS.handball,
    backgroundColor: '#E0A567', // Madera clara
    renderSVG: renderHandballCourt
  },
  volleyball: {
    type: 'volleyball',
    name: 'Cancha de Voleibol',
    dimensions: COURT_DIMENSIONS.volleyball,
    backgroundColor: '#D4A373',
    renderSVG: () => '<g></g>' // Placeholder
  },
  futsal: {
    type: 'futsal',
    name: 'Cancha de Fútbol Sala',
    dimensions: COURT_DIMENSIONS.futsal,
    backgroundColor: '#C17B3A',
    renderSVG: () => '<g></g>' // Placeholder
  }
};

/**
 * Helper function para obtener una plantilla de cancha
 */
export function getCourtTemplate(type: CourtType): CourtTemplate {
  return COURT_TEMPLATES[type];
}

/**
 * Helper function para renderizar una cancha con opciones
 */
export function renderCourt(type: CourtType, options?: CourtRenderOptions): string {
  const template = getCourtTemplate(type);
  return template.renderSVG(options);
}
