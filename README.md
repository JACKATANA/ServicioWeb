Config Service API: Servicio de Gestión de Configuración Dinámica¡Bienvenido al Config Service API! Este proyecto implementa un servicio RESTful de gestión de configuración dinámica. Su propósito es desacoplar variables de entorno, feature flags, y credenciales de las aplicaciones cliente, permitiendo la actualización de configuraciones sin necesidad de reiniciar la aplicación.Está construido con .NET (ASP.NET Core) y utiliza Docker Compose para orquestar la API junto con su base de datos PostgreSQL en un entorno multi-contenedor.🔗 RepositorioPuedes encontrar el código fuente completo y los archivos de configuración en:https://github.com/JACKATANA/ServicioWeb.git🚀 Tecnologías PrincipalesComponenteTecnologíaPropósitoBackend API.NET (ASP.NET Core)Exponer la API RESTful y gestionar la lógica de negocio.Base de DatosPostgreSQLPersistir la información de entornos y variables de configuración.OrquestaciónDocker y Docker ComposeAutomatizar la construcción, el despliegue y la interconexión de los servicios.⚙️ Guía de Configuración y DespliegueSigue estos pasos para levantar el servicio de API y la base de datos en tu entorno local.1. Requisitos PreviosAsegúrate de tener instalado y configurado:Docker Desktop (incluye Docker Engine y Docker Compose).Un editor de texto.2. Clonar el RepositorioClona este repositorio en tu máquina local y navega al directorio del proyecto:git clone [https://github.com/JACKATANA/ServicioWeb.git](https://github.com/JACKATANA/ServicioWeb.git)
cd ServicioWeb/
# Nota: La carpeta que contiene el docker-compose.yaml y la aplicación es ServicioWeb/ConfigServiceAPI/
# Por favor, ajusta tu ruta si la estructura es diferente.

3. Crear el Archivo de Variables de Entorno (.env)El servicio utiliza un archivo .env para gestionar las credenciales de la base de datos y la autenticación de la API.⚠️ ADVERTENCIA DE SEGURIDAD: Este archivo contiene secretos. DEBES añadir .env a tu archivo .gitignore para evitar subirlo al repositorio.Crea un nuevo archivo llamado .env en el directorio donde se encuentra tu docker-compose.yaml (probablemente ServicioWeb/ConfigServiceAPI/) y añade la siguiente estructura, reemplazando los valores de ejemplo con tus propios secretos REALES:# Variables de Entorno
# ----------------------------------------------

# --- Base de datos PostgreSQL ---
DB_PORT=5432
DB_HOST=postgres
DB_NAME=configdb
DB_USER=miusuario
DB_PASSWORD=TU_CONTRASEÑA_SECRETA_REAL

# --- API (ConfigServiceAPI) ---
API_PORT=8080
BASIC_AUTH_USER=admin
BASIC_AUTH_PASS=TU_SECRETO_AUTH_BASICA

4. Construir y Ejecutar el ServicioPara construir las imágenes de Docker y levantar todos los servicios (API y PostgreSQL):Ubicarse en el directorio principal donde esté el docker-compose.yaml.Ejecutar el comando de despliegue:docker-compose up --build -d

up: Crea y arranca los contenedores.--build: Fuerza la reconstrucción de la imagen de .NET.-d: Ejecuta los contenedores en segundo plano (detached mode).5. Acceso y Documentación del ServicioUna vez que los contenedores estén operativos (verifica su estado con docker-compose ps):API Base URL: http://localhost:${API_PORT} (ej. http://localhost:8080)Documentación (OpenAPI/Swagger): Accede a la documentación interactiva en http://localhost:${API_PORT}/swagger (depende de la configuración de tu proyecto .NET).🛑 Apagar y Limpiar el EntornoPara detener los servicios y eliminar contenedores, redes y, muy importante, los volúmenes de datos de PostgreSQL (borrando la base de datos):docker-compose down -v

down: Detiene y elimina los contenedores y la red por defecto.-v: Elimina el volumen pgdata, forzando una inicialización limpia en el próximo up.📘 Endpoints PrincipalesLa aplicación sigue una estructura RESTful para la gestión de recursos:RecursoMétodo HTTPURLDescripciónHealth CheckGET/status/Verifica el estado del servicio (pong).EntornosGET/enviroments/Lista todos los entornos (con paginación).VariablesPOST/enviroments/{env_name}/variablesCrea una nueva variable para un entorno.Consumo MasivoGET/enviroments/{env_name}.jsonEndpoint clave: Devuelve la configuración completa de un entorno como un JSON plano.
