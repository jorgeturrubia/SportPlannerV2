import { ExerciseAnimation, AnimationBuilder } from '../models/exercise-animation.model';

/**
 * Ejercicio Demo: Dribble entre conos + Pase + Tiro
 * 
 * Secuencia:
 * 1. Jugador 1 dribblea entre 3 conos (zigzag)
 * 2. Pasa el balón a Jugador 2
 * 3. Jugador 2 recibe y tira a canasta
 * 4. Jugador 1 corre a rebote
 */
export function createBasketballDribbleDrill(): ExerciseAnimation {
  const builder = new AnimationBuilder();

  // Frame 0: Posición inicial (0-500ms)
  builder.addFrame(500, (frame) => {
    // Jugadores
    builder.addPlayer(frame, 'player1', 15, 50, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    // Conos en zigzag
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    // Balón con jugador 1
    builder.addBall(frame, 15, 50);
    
    builder.addAnnotation(frame, 'Posición inicial', 50, 10, 1500);
  });

  // Frame 1: P1 empieza a driblear hacia cono 1 (500-1500ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 20, 48, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 20, 48);
    
    builder.addAction(frame, { type: 'dribble', playerId: 'player1' });
    builder.addAnnotation(frame, 'Dribble hacia cono 1', 20, 35, 1000);
  });

  // Frame 2: P1 rodea cono 1 (1500-2500ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 25, 43, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#EF4444'); // Destacar cono activo
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 25, 43);
    
    builder.addAction(frame, { type: 'dribble', playerId: 'player1' });
  });

  // Frame 3: P1 se mueve hacia cono 2 (2500-3500ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 30, 52, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 30, 52);
    
    builder.addAction(frame, { type: 'dribble', playerId: 'player1' });
    builder.addAnnotation(frame, 'Cambio de dirección', 30, 40, 1000);
  });

  // Frame 4: P1 rodea cono 2 (3500-4500ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 35, 57, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#EF4444'); // Destacar
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 35, 57);
    
    builder.addAction(frame, { type: 'dribble', playerId: 'player1' });
  });

  // Frame 5: P1 se mueve hacia cono 3 (4500-5500ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 40, 48, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 40, 48);
    
    builder.addAction(frame, { type: 'dribble', playerId: 'player1' });
  });

  // Frame 6: P1 rodea cono 3 (5500-6500ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 45, 43, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#EF4444'); // Destacar
    
    builder.addBall(frame, 45, 43);
    
    builder.addAction(frame, { type: 'dribble', playerId: 'player1' });
    builder.addAnnotation(frame, 'Último cono', 45, 30, 1000);
  });

  // Frame 7: P1 prepara el pase (6500-7000ms)
  builder.addFrame(500, (frame) => {
    builder.addPlayer(frame, 'player1', 50, 45, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 50, 45);
    
    builder.addAnnotation(frame, 'Preparar pase', 50, 32, 500);
  });

  // Frame 8: Pase en el aire (7000-8000ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 50, 45, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    // Balón interpolado entre P1 y P2
    builder.addBall(frame, 62.5, 37.5);
    
    builder.addAction(frame, { 
      type: 'pass', 
      playerId: 'player1',
      target: { x: 75, y: 30 }
    });
    builder.addAnnotation(frame, '¡Pase!', 62.5, 25, 1000);
  });

  // Frame 9: P2 recibe el balón (8000-8500ms)
  builder.addFrame(500, (frame) => {
    builder.addPlayer(frame, 'player1', 52, 45, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 75, 30, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 75, 30);
    
    builder.addAction(frame, { type: 'receive', playerId: 'player2' });
    builder.addAnnotation(frame, 'Recepción', 75, 20, 500);
  });

  // Frame 10: P2 prepara el tiro (8500-9500ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 55, 45, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 78, 28, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 78, 28);
    
    builder.addAnnotation(frame, 'Preparar tiro', 78, 18, 1000);
  });

  // Frame 11: Tiro a canasta (9500-11000ms)
  builder.addFrame(1500, (frame) => {
    builder.addPlayer(frame, 'player1', 60, 50, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 78, 28, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    // Balón hacia la canasta derecha (x=97, y=50 según SVG del template)
    builder.addBall(frame, 95, 48);
    
    builder.addAction(frame, { 
      type: 'shoot', 
      playerId: 'player2',
      target: { x: 97, y: 50 }
    });
    builder.addAnnotation(frame, '¡Tiro!', 85, 38, 1500);
  });

  // Frame 12: P1 corre a rebote (11000-12000ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 70, 45, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 78, 28, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 96, 50);
    
    builder.addAction(frame, { type: 'move', playerId: 'player1' });
    builder.addAnnotation(frame, 'Rebote posicional', 70, 35, 1000);
  });

  // Frame 13: Posición final (12000-13000ms)
  builder.addFrame(1000, (frame) => {
    builder.addPlayer(frame, 'player1', 80, 40, 'P1', '#3B82F6');
    builder.addPlayer(frame, 'player2', 78, 28, 'P2', '#10B981');
    
    builder.addCone(frame, 'cone1', 25, 45, '#F59E0B');
    builder.addCone(frame, 'cone2', 35, 55, '#F59E0B');
    builder.addCone(frame, 'cone3', 45, 45, '#F59E0B');
    
    builder.addBall(frame, 96, 50);
    
    builder.addAnnotation(frame, '¡Ejercicio completado!', 50, 50, 2000);
  });

  return builder.build({
    id: 'basketball-dribble-drill-001',
    name: 'Dribble entre conos + Pase + Tiro',
    description: 'Ejercicio completo que combina control de balón, pase preciso y finalización',
    sport: 'basketball',
    court: {
      type: 'basketball',
      renderOptions: {
        showLines: true,
        showThreePointLine: true,
        showKeyArea: true,
        showCenterCircle: true,
        lineColor: '#FFFFFF',
        opacity: 1
      },
      backgroundColor: '#C17B3A' // Parquet color
    },
    metadata: {
      difficulty: 'intermediate',
      focus: ['dribbling', 'passing', 'shooting', 'teamwork'],
      playerCount: 2
    }
  });
}
