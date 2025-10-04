export interface BasicAthlete {
  id: number;
  name: string;
  sport: string;
  level: string;
  status: string;
}

export interface AdvancedAthlete {
  id: number;
  name: string;
  email: string;
  sport: string;
  level: string;
  age: number;
  weight: number;
  height: number;
  country: string;
  status: string;
  lastSession: string;
  totalSessions: number;
}

const athleteNames = [
  'Carlos Rodríguez', 'Ana Martínez', 'Luis García', 'María López', 'Pedro Sánchez',
  'Laura Fernández', 'Miguel Torres', 'Carmen Ruiz', 'Juan Díaz', 'Isabel Moreno',
  'David Jiménez', 'Elena Álvarez', 'Francisco Romero', 'Paula Navarro', 'Antonio Gil',
  'Sofía Castro', 'Roberto Muñoz', 'Cristina Pérez', 'Javier Hernández', 'Lucía Ramos',
  'Fernando Vega', 'Patricia Molina', 'Sergio Ortiz', 'Beatriz Delgado', 'Andrés Vargas',
  'Valentina Suárez', 'Diego Mendoza', 'Gabriela Ríos', 'Pablo Gutiérrez', 'Carolina Flores',
  'Raúl Paredes', 'Natalia Campos', 'Alberto Silva', 'Daniela Cortés', 'Marcos Reyes',
  'Adriana León', 'Emilio Santos', 'Victoria Núñez', 'Gustavo Medina', 'Camila Ramírez',
  'Héctor Aguilar', 'Mariana Cruz', 'Óscar Morales', 'Alejandra Guerrero', 'Ricardo Herrera',
  'Sandra Castillo', 'Manuel Ibáñez', 'Teresa Vázquez', 'Jorge Domínguez', 'Mónica Cabrera'
];

const sports = [
  'Football', 'Basketball', 'Tennis', 'Swimming', 'Athletics',
  'Cycling', 'Volleyball', 'Boxing', 'Rugby', 'Handball',
  'Table Tennis', 'Badminton', 'Golf', 'Hockey', 'Martial Arts'
];

const levels = ['Beginner', 'Intermediate', 'Advanced', 'Professional', 'Elite'];

const statuses = ['Active', 'Inactive', 'On Leave', 'Injured', 'Recovery'];

const countries = [
  'Spain', 'Mexico', 'Argentina', 'Colombia', 'Chile', 'Peru',
  'USA', 'Brazil', 'Uruguay', 'Venezuela', 'Ecuador', 'Bolivia',
  'Paraguay', 'Costa Rica', 'Panama', 'Guatemala', 'Honduras', 'Nicaragua'
];

export function generateBasicAthletes(count: number): BasicAthlete[] {
  return Array.from({ length: count }, (_, i) => ({
    id: i + 1,
    name: athleteNames[i % athleteNames.length],
    sport: sports[Math.floor(Math.random() * sports.length)],
    level: levels[Math.floor(Math.random() * levels.length)],
    status: statuses[Math.floor(Math.random() * statuses.length)]
  }));
}

export function generateAdvancedAthletes(count: number): AdvancedAthlete[] {
  return Array.from({ length: count }, (_, i) => {
    const name = athleteNames[i % athleteNames.length];
    const lastName = name.split(' ')[1] || 'Athlete';
    const firstName = name.split(' ')[0] || 'User';

    return {
      id: i + 1,
      name,
      email: `${firstName.toLowerCase()}.${lastName.toLowerCase()}@sportplanner.com`,
      sport: sports[Math.floor(Math.random() * sports.length)],
      level: levels[Math.floor(Math.random() * levels.length)],
      age: Math.floor(Math.random() * 30) + 18,
      weight: Math.floor(Math.random() * 40) + 60,
      height: Math.floor(Math.random() * 40) + 160,
      country: countries[Math.floor(Math.random() * countries.length)],
      status: statuses[Math.floor(Math.random() * statuses.length)],
      lastSession: new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
      totalSessions: Math.floor(Math.random() * 200) + 1
    };
  });
}

// Pre-generated datasets
export const BASIC_ATHLETES_50 = generateBasicAthletes(50);
export const ADVANCED_ATHLETES_100 = generateAdvancedAthletes(100);
