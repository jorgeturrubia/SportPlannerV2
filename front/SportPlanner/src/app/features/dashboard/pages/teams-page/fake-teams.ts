export const FAKE_TEAMS = [
  {
    id: 't1',
    name: 'Águilas FC',
    color: '#ef4444',
    description: 'Equipo juvenil con enfoque en técnica y táctica.',
    members: Array.from({ length: 12 }, (_, i) => ({ id: `m${i+1}`, name: `Jugador ${i+1}` })),
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 30)
  },
  {
    id: 't2',
    name: 'Leones Club',
    color: '#f97316',
    description: 'Club amateur con pasión por competir.',
    members: Array.from({ length: 8 }, (_, i) => ({ id: `m${i+1}`, name: `Jugador ${i+1}` })),
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 60)
  },
  {
    id: 't3',
    name: 'Trueno Basket',
    color: '#10b981',
    description: 'Equipo de baloncesto mixto.',
    members: Array.from({ length: 6 }, (_, i) => ({ id: `m${i+1}`, name: `Jugador ${i+1}` })),
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 10)
  },
  {
    id: 't4',
    name: 'Halcones',
    color: '#06b6d4',
    description: 'Formación de base y desarrollo.',
    members: Array.from({ length: 4 }, (_, i) => ({ id: `m${i+1}`, name: `Jugador ${i+1}` })),
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 90)
  },
  {
    id: 't5',
    name: 'Titanes',
    color: '#7c3aed',
    description: 'Equipo competitivo, alta exigencia.',
    members: Array.from({ length: 15 }, (_, i) => ({ id: `m${i+1}`, name: `Jugador ${i+1}` })),
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 200)
  },
  {
    id: 't6',
    name: 'Orcas Azul',
    color: '#2563eb',
    description: 'Equipo recreativo para amigos.',
    members: Array.from({ length: 3 }, (_, i) => ({ id: `m${i+1}`, name: `Jugador ${i+1}` })),
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 5)
  }
];