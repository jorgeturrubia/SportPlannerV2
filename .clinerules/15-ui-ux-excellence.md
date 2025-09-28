# 15 - Reglas de UI/UX Técnico

## Layout y Espaciado

-   **15.1: Sistema de Rejilla (Grid)**
    -   **Regla**: Todos los componentes y layouts deben alinearse a una rejilla basada en `4px`.
    -   **Implementación**: Utilizar exclusivamente las clases de espaciado y tamaño de Tailwind (`p-4`, `m-8`, `gap-2`, `w-32`). No usar valores arbitrarios como `margin: 3px`.

-   **15.2: Jerarquía de Elevación (Z-Depth)**
    -   **Regla**: La profundidad debe usarse para indicar la jerarquía de los elementos en la pantalla.
    -   **Implementación**: Los elementos interactivos superpuestos (modales, menús, pop-ups) deben usar una elevación de sombra mayor (`shadow-lg`, `shadow-xl`) que el contenido estático.

## Interacción y Estado de Componentes

-   **15.3: Estados de Interacción**
    -   **Regla**: Todo elemento interactivo debe tener estilos visualmente distintos para cada uno de sus estados.
    -   **Implementación**: Implementar obligatoriamente los estilos para los estados `:hover`, `:focus`, `:active`, y `:disabled`. El estado `:focus` debe ser claramente visible para la navegación con teclado.

-   **15.4: Feedback de Operaciones Asíncronas**
    -   **Regla**: Las operaciones que no son instantáneas deben proveer feedback claro sobre su estado.
    -   **Implementación**:
        -   Usar `mat-progress-spinner` o `mat-progress-bar` durante la ejecución de la operación.
        -   Usar el `MatSnackBar` para comunicar el resultado final (éxito o error).

## Formularios

-   **15.5: Validación en Tiempo Real**
    -   **Regla**: La validación de los campos de un formulario debe ejecutarse a medida que el usuario escribe.
    -   **Implementación**: Usar `ReactiveFormsModule` de Angular para mostrar mensajes de error específicos por campo tan pronto como el validador falle.

-   **15.6: Reducción de Carga Cognitiva en Formularios**
    -   **Regla**: Los formularios complejos deben simplificarse y guiarse.
    -   **Implementación**:
        -   Formularios con más de 7 campos o varias secciones deben dividirse en pasos usando el componente `MatStepper`.
        -   Para campos que requieren seleccionar de una lista larga (ej: atletas), se debe usar el componente `mat-autocomplete`.

## Rendimiento Percibido

-   **15.7: Carga de Vistas de Datos**
    -   **Regla**: El usuario nunca debe ver una pantalla en blanco mientras se cargan los datos.
    -   **Implementación**: Implementar **Skeleton Screens** (contenedores de carga con la forma del contenido final) para todas las vistas principales que dependan de una carga de datos asíncrona.

-   **15.8: Optimización de Contenido Visual (CLS)**
    -   **Regla**: La carga de imágenes no debe causar un reajuste del layout (Cumulative Layout Shift).
    -   **Implementación**: Las imágenes (`<img>`) y contenedores de iframes deben tener sus atributos `width` y `height` definidos. Usar el atributo `loading="lazy"` para imágenes fuera del viewport inicial.

g## Accesibilidad (A11y)

-   **15.9: Estándar Mínimo de Accesibilidad**
    -   **Regla**: El objetivo mínimo de la aplicación es el cumplimiento del estándar **WCAG 2.1 Nivel AA**.
    -   **Implementación**:
        -   Usar HTML semántico (`<main>`, `<nav>`, etc.).
        -   Asegurar que todos los componentes interactivos son completamente operables con teclado.
        -   Los botones que solo contienen un icono deben usar el atributo `aria-label` para describir su acción.
