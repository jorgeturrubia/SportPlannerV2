# Testing Standards

## Cobertura de Testing
- **Unit tests**: Cobertura >80% en capas Application/Domain
- **Integration tests**: Para features principales y APIs críticas
- **E2E tests**: Para flujos de usuario completos
- **Component tests**: Para componentes Angular complejos

## Estructura de Tests
```
src/
├── app/
│   ├── features/
│   │   └── training/
│   │       ├── training.component.spec.ts
│   │       ├── training.service.spec.ts
│   │       └── training.integration.spec.ts
├── tests/
│   ├── e2e/
│   ├── fixtures/
│   └── mocks/
```

## Unit Testing
- **Framework**: Jest + Angular Testing Utilities
- **Naming**: `describe('ClassName', () => { it('should do something', () => {}) })`
- **AAA Pattern**: Arrange, Act, Assert
- **Mocking**: Mock dependencies externas, servicios HTTP, y localStorage

## Integration Testing
- **Scope**: Interacción entre servicios y componentes
- **Backend**: Mocking de APIs y base de datos
- **Servicios externos**: Mock de Supabase, dispositivos, etc.

## E2E Testing
- **Framework**: Playwright o Cypress
- **Escenarios**: Registro, login, creación de entrenamientos, visualización de progreso
- **Data**: Base de datos de test aislada
- **CI/CD**: Ejecución automática en pipeline

## Testing para SportPlanner
```typescript
// Ejemplo: Training Service Test
describe('TrainingService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TrainingService,
        { provide: SupabaseClient, useValue: mockSupabaseClient }
      ]
    });
  });

  it('should create training plan', async () => {
    // Arrange
    const trainingData = { name: 'Test Plan', duration: 60 };
    
    // Act
    const result = await service.createTraining(trainingData);
    
    // Assert
    expect(result).toBeDefined();
    expect(result.name).toBe('Test Plan');
  });
});
```

## Mocking Strategy
- **Supabase**: Mock del cliente y operaciones CRUD
- **Dispositivos**: Mock de sensores y wearables
- **APIs externas**: Mock de servicios de terceros
- **LocalStorage/SessionStorage**: Mock del browser storage

## Continuous Testing
- Tests obligatorios antes de merge
- Coverage report en cada PR
- Tests de regresión automáticos
- Performance testing para operaciones críticas
