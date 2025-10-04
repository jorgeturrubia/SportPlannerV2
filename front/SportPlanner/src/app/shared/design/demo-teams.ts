export const DEMO_TEAMS = [
  {
    id: 'team-1',
    name: 'Falcons',
    description: 'Equipo juvenil - velocidad y agilidad',
    color: '#2563eb',
    members: [{ name: 'Ana' }, { name: 'Luis' }, { name: 'Marta' }],
    createdAt: new Date().toISOString(),
    activityScore: 0.72
  },
  {
    id: 'team-2',
    name: 'Tigers',
    description: 'Equipo senior - fuerza y táctica',
    color: '#ef4444',
    members: [{ name: 'Carlos' }, { name: 'Rosa' }, { name: 'Diego' }, { name: 'Lucía' }],
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 10).toISOString(),
    activityScore: 0.55
  },
  {
    id: 'team-3',
    name: 'Comets',
    description: 'Velocidad en pista',
    color: '#10b981',
    members: [{ name: 'Pedro' }, { name: 'Sofía' }],
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 30).toISOString(),
    activityScore: 0.88
  },
  {
    id: 'team-4',
    name: 'Dragons',
    description: 'Entrenamiento mixto',
    color: '#8b5cf6',
    members: [{ name: 'Eva' }, { name: 'Tom' }, { name: 'Noa' }, { name: 'Iker' }, { name: 'Mia' }],
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 60).toISOString(),
    activityScore: 0.33
  },
  {
    id: 'team-5',
    name: 'Waves',
    description: 'Natación y resistencia',
    color: '#06b6d4',
    members: [{ name: 'Bruno' }, { name: 'Clara' }, { name: 'Nuria' }],
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 5).toISOString(),
    activityScore: 0.61
  },
  {
    id: 'team-6',
    name: 'Knights',
    description: 'Estrategia y defensa',
    color: '#f97316',
    members: [{ name: 'Hugo' }, { name: 'Alicia' }, { name: 'Raúl' }, { name: 'Iris' }],
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 15).toISOString(),
    activityScore: 0.47
  }
];
