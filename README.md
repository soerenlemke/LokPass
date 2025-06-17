# LokPass – Password Manager

**LokPass** is a cross-platform, password manager and a personal learning project.  
The backend is built with **.NET** and the application is designed to be extended with multiple types of **frontends** (such as CLI or desktop GUI).

---

## Project Goals

The goal of LokPass is to create an extensible application that:

- Runs entirely offline – no cloud or external services (syncing solutions may be added in the future)
- Stores passwords using modern cryptographic standards
- Is modular and flexible in how users interact with it (UI, CLI, API)
- Is fully open source and auditable
- Serves as a practical learning project for .NET development, focusing on:
    - System-level programming
    - Architecture
    - Cryptography and data storage

---

## Backend (.NET)

The backend is written in **.NET** and provides:

- Password storage (e.g. AES-GCM encryption, Argon2 hashing)
- Data persistence (planned: SQLite)
- Local API endpoints (planned: REST or gRPC)
- Clear separation of core logic and interfaces for easy extension

---

## Frontend

Starting:

- Desktop UI (Avalonia)

To be defined – possible options include:

- Command Line Interface (CLI)
- Web-based or mobile clients (future consideration)

---

## License

This project is licensed under the **MIT License**.
