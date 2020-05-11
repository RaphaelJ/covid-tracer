# CovidTracer

CovidTracer es una aplicación de rastreo de contacto **descentralizada** y **anónima** diseñada para la pandemia actual del COVID-19.

CovidTracer le notifica al usuario de cualquier contacto cercano con otros usuarios diagnosticados con COVID-19. 

![Android](screenshots/screenshot-android.png) ![iOS](screenshots/screenshot-ios.png)

CovidTracer usa Bluetooth y técnicas criptográficas para proteger la privacidad del usuario. Los usuarios de la app no comparten informacion personal. Jamas se mantiene ningún registro de la ubicación GPS.

CovidTracer sigue las [recomendaciones de rastreo de contacto](https://www.eff.org/deeplinks/2020/04/challenge-proximity-apps-covid-19-contacto-tracing) de la Electronic Frontier Foundation.

CovidTracer es un software libre y gratuito(GPLv3).

## Descarga e Instalación

Google y Apple [actualmente no permiten](https://www.theverge.com/2020/3/5/21167102/apple-google-coronavirus-iphone-apps-android-misinformation-reject-ban) ninguna aplicación relacionada al coronavirus que no este relacionada con organizaciones de la salud reconocidas o gobiernos o sus tiendas. La aplicación aun se puede instalar por medios alternativos:

En **Android**, se puede descargar un APK instalable [aqui](https://github.com/RaphaelJ/covid-tracer/releases/download/v0.1.1/covidtracer_0.1.1.apk).

En **iOS**, solo los desarrolladores registrados de Apple pueden compilar, firmar e instalar la app en sus iPhones. Esto se puede realizar mediante la clonacion del repositorio y mediante el uso de [Visual Studio para macOS](https://visualstudio.microsoft.com/vs/mac/). La app de iOS no soporta todas las características aun. 

## Preguntas Frecuentes

- **Cuales son las capacidades de privacidad en CovidTracer?**
CovidTracer usa tecnicas criptograficas similares a las usadas por e-commerce y crypto-monedas. Estas proveen altos niveles de privacidad que evitan que cualquiera asocie el uso de esta app con cualquiera de tus datos personales (ubicación, nombre...).

- **Cuando debería usar CovidTracer?**
Para eficiencia máxima, abre la app y déjala ejecutarse en 2do plano cuando interactúas con personas ajenas a tu hogar (transportes, oficina, tiendas, actividades en el exterior...).

- **Que hago cuando la aplicación detecta un contacto de alto riesgo?**
Por favor informa a la autoridad competente tan pronto como se detecte el contacto de alto-riesgo. La app no notifica a ninguna organización gubernamental ni de la salud sobre el caso positivo.

- **Por que pide acceso a la ubicación la aplicación de Android?**
La aplicación de Android debe [pedir el permiso de ubicacion](https://developer.android.com/guide/topics/connectivity/bluetooth#Permissions) para acceder a algunas capacidades bluetooth, como el escaneo. CovidTracer no utiliza ni registra.

- **Como puedo remover cualquier información recolectada por la app?**
Des-instalar la aplicación de tu smartphone va a borrar toda la informacion que recolecto la aplicacion. Si alguna vez te reportaste a ti mismo como un caso positivo, algunos datos (anónimos) asociados a tu periodo infeccioso aun van a estar disponibles a otros usuarios de la app.

- **Me gusta el proyecto. Como puedo ayudar?** 
Estoy desarrollando la aplicación como un proyecto personalcon pocos recursos. Recibiría con mucho gusto cualquier ayuda.
    - Si hablas un lenguaje extranjero, puedes ayudar traduciendo [uno de los archivos de localización](CovidTracer/Resx/);
    - Si tienes habilidades en ciencia computacional y/o cryptografia, no dudes en leer los detalles técnicos y en proveer feedback;
    - Si eres un diseñador gráfico o de UX, puedes ayudar a mejorar algunos de los componentes de la UI de la aplicación (onboarding, iconos...);
    - Crear una aplicación (mayormente) distribuida de rastreo de contacto es posible. Por favor presiona a tu gobierno local a tener la privacidad en cuenta si están desarrollando sus propios sistemas de rastreo de contactos.

## Detalles tecnicos

### Overview

La app constantemente envía(broadcasts) un identificador de 20 bytes mediante Bluetooth Low Energy (BLE) a dispositivos cercanos. Este identificador se genera aleatoriamente (y por lo tanto no puede ser relacionado con información personal) y es renovado cada hora (evitando el rastreo por largo tiempo). Usuarios cercanos de CovidTracer constantemente registran estos identificadores en una base de datos localizada en sus dispositivos. Estos identificadores no se comparten con ningún servidor central ni entidad.

Si el usuario alguna vez se reporta a si mismo con positivo para SARS-CoV-2, los identificadores generados horalmente correspondientes al periodo infeccioso (16 días) son anónimamente publicados en un servidor central. Otros usuarios de la app pueden luego comparar estos identificadores con los que registraron en los últimos días.

### Detalles

Cuando la app se abre por primera vez, se [genera una llave de 256 bits](CovidTracer/Models/Keys/TracerKey.cs#L54) usando un generador de números criptográficos aleatorios:

    TracerKey = RNG()

Esta llave no va a ser compartida con ningún otro usuario de la app, pero sera usada para crear llaves diarias y horales.

Cada día (UTC), [se deriva una nueva llave de 256 bits](CovidTracer/Models/Keys/TracerKey.cs#L80) de la `TracerKey` usando una funcion *SHA-256 HMAC*, en conjunto con la fecha actual (como una cadena de caracteres ISO 8601):

    DailyKey = HMAC-SHA256(TracerKey, CurrentDate('YYYY-MM-DD'))
    
La `TracerKey` original no puede ser derivada desde la `DailyKey`. 

El identificador actual enviado por bluetooth [es derivado](CovidTracer/Models/Keys/DailyTracerKey.cs#L49) cada hora (UTC) de la llave del día actual y la hora actual. ya que las características de BLE están limitadas a 20 bytes, esta llave tambien es truncada:

    CurrentKey = TRUNCATE(HMAC-SHA256(DailyKey, CurrentTime('YYYY-MM-DDTHH'))

Si un usuario se reporta a si mismo como positivo para SARS-CoV-2, todos los identificadores diarios usados/a ser usados durante el periodo infeccioso van a ser [compartidos con un servidor central](https://covid-tracer-backend.herokuapp.com/cases.json) (desde 5 días antes de que aparescan los síntomas, hasta 11 días despues). Otras instancias de la aplicación pueden despues derivar llaves generadas horalmente durante el periodo infeccioso, y potencialmente coincidir con cualquier contacto que allan tenido previamente. 

Medidas adicionales para mejorar la privacidad:

- Las llaves de rastreo de contactos son automáticamente removidas del dispositivo luego de 15 días;
- El backend retorna llaves diarias en orden alfabético, y solo las publica cada 12 horas. Esto hace que sea mas difícil asociar múltiples llaves diarias con un solo usuario;
- El backend no publica llaves diarias de fechas futuras, y las aplicacionessolo coinciden contactos que ocurrieron en el dia asociado con la llave. Esto evita la impersonificacion del usuario; 
- La calidad de la señal Bluetooth es usada para evaluar la proximidad de dispositivos cercanos. El algoritmo solo para registrar identificadores de dispositivos ubicados en la misma habitación;
- El backend implementa limites de ratio estrictos en el reportaje;
- Toda la comunicación con el backend se hace mediante HTTPS;
- El backend [esta disponible](https://github.com/RaphaelJ/covid-tracer-backend) como software libre y gratuito.

Opciones mas avanzadas de diagnostico e información de debugging se puede obtener directamente desde la aplicación haciendo click 10 veces en la "tracer key ID" en la pagina de *Acerca de*.

## Agradecimientos especiales

Estos usuarios proveyeron la traducción en los siguientes lenguajes:

- Croatian/Hrvatski: [micimacahaca](https://old.reddit.com/user/micimacahaca);
- Holandés: [vlammuh](https://www.reddit.com/user/vlammuh) y [bavoceulemans](https://github.com/bavoceulemans);
- Brazileño/Portugues: [Raphael Salomao](https://github.com/raphaelsalomao3);
- Español: [Barraguesh](https://github.com/Barraguesh) y [PinkDev1](https://github.com/PinkDev1)