# Clean Code Principles

## Estructura y Organización
- Mantén clases y archivos cortos (<300 líneas)
- No mezcles UI y lógica de negocio en el mismo componente
- Separa responsabilidades claramente: presentación, lógica de negocio y acceso a datos

## Principios SOLID
- **Single Responsibility**: Cada clase/función debe tener una única razón para cambiar
- **Open/Closed**: Abierto para extensión, cerrado para modificación
- **Liskov Substitution**: Los objetos derivados deben poder reemplazar a sus clases base
- **Interface Segregation**: Los clientes no deben depender de interfaces que no usan
- **Dependency Inversion**: Depende de abstracciones, no de concreciones

## DRY (Don't Repeat Yourself)
- Extrae código duplicado a funciones/servicios reutilizables
- Usa constantes para valores que se repiten
- Crea composables/servicios para lógica compartida

## Legibilidad y Mantenimiento
- Código autodocumentado con nombres descriptivos
- Comentarios solo cuando el "por qué" no sea obvio
- Funciones pequeñas con un propósito claro
- Evita anidamiento excesivo (máximo 3 niveles)

## Gestión de Errores
- Manejo explícito de errores, no los ignores
- Usa tipos específicos de error cuando sea posible
- Logs informativos para debugging
