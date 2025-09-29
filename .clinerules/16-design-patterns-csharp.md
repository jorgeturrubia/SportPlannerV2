# 16 - Guía Práctica de Patrones de Diseño en C#

Esta guía te ayuda a decidir **cuándo** aplicar un patrón de diseño. Antes de implementar una nueva funcionalidad, revisa esta guía para determinar si un patrón puede mejorar la estructura, mantenibilidad y escalabilidad de tu código.

---

## 🤔 ¿Cuándo debo usar un patrón de diseño?

Usa esta guía como un mapa. Describe el problema que estás tratando de resolver y busca un patrón que se ajuste a ese problema.

- **¿Necesitas crear objetos de forma flexible o desacoplada?** -> Revisa los **Patrones Creacionales**.
- **¿Necesitas organizar clases y objetos en estructuras más grandes y flexibles?** -> Revisa los **Patrones Estructurales**.
- **¿Necesitas gestionar algoritmos, responsabilidades y la comunicación entre objetos?** -> Revisa los **Patrones de Comportamiento**.

---

## 🎨 Patrones Creacionales

Se centran en la creación de objetos de manera controlada y desacoplada.

### **Factory Method**
- **Cuándo usarlo**: Cuando no sabes de antemano el tipo exacto de objeto que tu código debe crear. Delega la responsabilidad de la instanciación a subclases.
- **Caso de uso**: Un sistema que necesita generar diferentes tipos de documentos (PDF, DOCX, TXT). La clase base `DocumentGenerator` tiene un método `CreateDocument()`, y las subclases `PdfGenerator`, `DocxGenerator` lo implementan para crear el objeto específico.

### **Abstract Factory**
- **Cuándo usarlo**: Cuando necesitas crear familias de objetos relacionados sin especificar sus clases concretas.
- **Caso de uso**: Para construir interfaces de usuario que deben soportar múltiples temas (Light, Dark). Una `UIFactory` abstracta define métodos como `CreateButton()` y `CreateCheckbox()`. `LightThemeFactory` y `DarkThemeFactory` implementan estos métodos para crear botones y checkboxes con el estilo del tema correspondiente.

### **Builder**
- **Cuándo usarlo**: Para construir objetos complejos paso a paso. Permite producir diferentes tipos y representaciones de un objeto utilizando el mismo código de construcción.
- **Caso de uso**: Crear un objeto `UserConfiguration` con muchas propiedades opcionales (username, avatar, theme, notifications, etc.). Un `UserConfigurationBuilder` permite establecer solo las propiedades necesarias de forma legible y encadenada: `new UserConfigurationBuilder().WithUsername("test").WithTheme("dark").Build()`.

### **Singleton**
- **Cuándo usarlo**: Cuando necesitas asegurarte de que una clase tenga una única instancia y proporcionar un punto de acceso global a ella.
- **PRECAUCIÓN**: Úsalo con moderación. Puede introducir acoplamiento global y dificultar las pruebas.
- **Caso de uso**: Una clase que gestiona la configuración de la aplicación (`AppSettings`) o un servicio de logging que debe ser accesible desde cualquier parte del sistema.

### **Prototype**
- **Cuándo usarlo**: Cuando el coste de crear un objeto es más alto que el de clonarlo. Permite copiar objetos existentes sin que el código dependa de sus clases.
- **Caso de uso**: En un juego, para crear múltiples enemigos del mismo tipo. En lugar de instanciar un nuevo objeto `Enemy` con todos sus atributos desde cero, clonas un objeto `Enemy` ya configurado.

---

## 🏗️ Patrones Estructurales

Tratan sobre cómo se componen y relacionan las clases y objetos.

### **Adapter**
- **Cuándo usarlo**: Para hacer que dos interfaces incompatibles trabajen juntas.
- **Caso de uso**: Tienes una librería externa que devuelve datos en XML, pero tu aplicación trabaja con JSON. Creas un `XmlToJsonAdapter` que envuelve la librería, recibe el XML y lo transforma a JSON antes de entregarlo a tu aplicación.

### **Decorator**
- **Cuándo usarlo**: Para añadir nuevas funcionalidades a objetos dinámicamente, envolviéndolos en objetos especiales que contienen esos comportamientos.
- **Caso de uso**: Un sistema de notificaciones. Tienes un notificador base que envía un email. Puedes "decorarlo" con un `SmsNotifierDecorator` y un `PushNotifierDecorator` para añadir el envío de SMS y notificaciones push sin modificar la clase original.

### **Facade**
- **Cuándo usarlo**: Para proporcionar una interfaz simplificada a un subsistema complejo (una librería, un framework, un conjunto de clases).
- **Caso de uso**: Una API para procesar pagos que requiere interactuar con múltiples servicios (validación, cargo, notificación). Creas una `PaymentFacade` con un único método `ProcessPayment()` que orquesta internamente todas las llamadas a los diferentes servicios.

### **Proxy**
- **Cuándo usarlo**: Para proporcionar un sustituto o intermediario de otro objeto para controlar el acceso a él.
- **Caso de uso**:
    - **Lazy Loading**: Un `ImageProxy` que solo carga la imagen real de alta resolución cuando se llama a su método `Display()`.
    - **Control de Acceso**: Un `UserProxy` que comprueba los permisos del usuario actual antes de permitir la ejecución de métodos sensibles en el objeto `User` real.

---

## 🏃 Patrones de Comportamiento

Se ocupan de la comunicación y la asignación de responsabilidades entre objetos.

### **Strategy**
- **Cuándo usarlo**: Cuando tienes varios algoritmos para una tarea y quieres que el cliente elija el algoritmo a utilizar en tiempo de ejecución.
- **Caso de uso**: Un sistema de compresión de ficheros. Defnes una interfaz `ICompressionStrategy` y creas implementaciones como `ZipCompressionStrategy` y `RarCompressionStrategy`. El cliente puede elegir qué estrategia usar para comprimir un fichero.

### **Observer**
- **Cuándo usarlo**: Para definir un mecanismo de suscripción que notifica a múltiples objetos sobre cualquier evento que le suceda al objeto que están observando.
- **Caso de uso**: En una arquitectura de eventos. Cuando el estado de un `Order` cambia (ej. a "Enviado"), notifica a todos los suscriptores (`EmailService`, `InventoryService`, `AnalyticsService`) para que realicen sus respectivas acciones.

### **Command**
- **Cuándo usarlo**: Para convertir una solicitud en un objeto independiente que contiene toda la información sobre la solicitud. Permite parametrizar métodos con diferentes solicitudes, y encolar o registrar solicitudes.
- **Caso de uso**: Implementar funcionalidades de "Deshacer/Rehacer". Cada acción del usuario (escribir, borrar, mover) se encapsula en un objeto `Command`. Una pila de comandos permite deshacer la acción ejecutando el método `Undo()` del último comando.

### **State**
- **Cuándo usarlo**: Cuando el comportamiento de un objeto cambia según su estado interno. El objeto parece cambiar de clase.
- **Caso de uso**: La gestión de un `Documento`. El objeto `Documento` puede estar en estado `Draft`, `InReview` o `Published`. Su comportamiento (ej. el método `Edit()`) cambia radicalicamente dependiendo del estado. En lugar de usar `if/else` masivos, el objeto `Documento` delega el comportamiento a un objeto de estado actual (`DraftState`, `ReviewState`).

### **Mediator**
- **Cuándo usarlo**: Para reducir las dependencias caóticas entre objetos. El patrón restringe las comunicaciones directas entre los objetos y los obliga a colaborar únicamente a través de un objeto mediador.
- **Caso de uso**: En un formulario complejo, los componentes (botones, checkboxes, text fields) no se comunican directamente entre sí. Se comunican con un `FormMediator`, que centraliza la lógica. Por ejemplo, si un checkbox se marca, notifica al mediador, y el mediador decide si un botón debe habilitarse.
