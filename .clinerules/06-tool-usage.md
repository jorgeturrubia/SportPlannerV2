# 06 - Uso de Herramientas del Agente

## Ejecución de Comandos de Larga Duración

### Problema
Cuando se utiliza la herramienta `run_shell_command` para ejecutar procesos que no terminan por sí solos (por ejemplo, un servidor de desarrollo, un watcher de archivos, etc.), el agente se queda bloqueado esperando a que el proceso finalice. Esto impide que el agente continúe con otras tareas.

### Solución
Para evitar el bloqueo, cualquier comando que inicie un proceso de larga duración **debe** ejecutarse en segundo plano (background).

Para hacer esto, simplemente añade el símbolo `&` al final del comando.

### Ejemplos

-   **Correcto (se ejecuta en segundo plano):**
    ```bash
    npm run start &
    ```
    ```bash
    dotnet watch run &
    ```

-   **Incorrecto (bloquea al agente):**
    ```bash
    npm run start
    ```

### Comportamiento Esperado
Después de iniciar un proceso en segundo plano, el agente debe continuar con el plan de trabajo. Si el objetivo era analizar la salida del proceso, el agente debe esperar un tiempo prudencial para que el proceso genere salida y luego proceder a analizarla, sin dejar de estar disponible para otras tareas.
