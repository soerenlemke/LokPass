# LokPass – Password Manager

![image](https://github.com/user-attachments/assets/75571a9f-a731-48b3-9abc-449c8db72d74)

**LokPass** is a cross-platform, password manager and a personal learning project.  

---

## Project Goals

The goal of LokPass is to create an extensible application that:

- Runs entirely offline – no cloud or external services (syncing solutions may be added in the future)
- Stores passwords using modern cryptographic standards
- Is modular and flexible in how users interact with it (UI, CLI, API)

---

## Backend (.NET)

The backend is written in **.NET** and provides:

- Password storage (e.g. AES-GCM encryption, Argon2 hashing)
- Data persistence (planned: SQLite)
- Local API endpoints (planned: REST or gRPC)

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
