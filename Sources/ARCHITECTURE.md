# Architecture
The architecture of the application is based on the principles of Clean Architecture, which emphasizes separation of concerns and promotes maintainability, testability, and scalability. The application is organized into several layers, each with its own responsibilities:
# Analyzer
| From ↓ / To → | Common | Domain | Application | Persistence | Presentation | Startup |
| --- | --- | --- | --- | --- | --- | --- |
| **Common** | ✔ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Domain** | ✔ | ✔ | ❌ | ❌ | ❌ | ❌ |
| **Application** | ✔ | ✔ | ✔ | ❌ | ❌ | ❌ |
| **Persistence** | ✔ | ✔ | ✔ | ✔ | ❌ | ❌ |
| **Presentation** | ✔ | ✔ | ✔ | ❌ | ✔ | ❌ |
| **Startup** | ✔ | ✔ | ✔ | ✔ | ✔ | ✔ |