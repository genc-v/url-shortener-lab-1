![alt](https://i.ibb.co/NYYLTPz/logo.png)

# URL Shortener

A feature-rich URL shortener built using nuxt, .NET 7, and CSS. This application allows users to shorten, search, and modify URLs. It can be run locally using XAMPP, via Docker, or accessed online at [bytely.xyz](https://bytely.xyz).

## Features

- **Shorten URLs:** Generate short URLs for easier sharing.
- **Search URLs:** Search and manage previously shortened URLs.
- **Modify URLs:** Update or customize shortened links.

- Account that is filled for testing:
  - email: test@test.com
  - passowrd: test

---

### Running with Docker

If you prefer to use Docker, follow these steps:

1. **Ensure Docker is Installed:**

   - Download and install [Docker](https://www.docker.com/).

2. **Clone the Repository:**

   ```bash
   git clone https://github.com/genc-v/url-shortener-lab-1.git
   ```

3. **Navigate to the Project Directory:**

   ```bash
   cd url-shortener-lab-1
   ```

4. **Run the Application Using Docker Compose:**

   ```bash
   docker compose up --build
   ```

   This will build and start the application in a containerized environment.

5. **Access the Application:**
   Open your browser and navigate to:
   ```
   http://localhost:3000
   ```

---

## Getting Started

### Running Locally the Front End

To run this project locally, follow these steps:

1. **Npm install**
   Go to cd frontend and run npm run install

2. **Change the env file for the backend**
   .env change the API to your local backend ip

3. **Run frontend**
   On terminal run npm run dev and go to localhost:3000 on your browser

### Running with backend

If you prefer to use Docker, follow these steps:

1. **Ensure Docker is Installed:**

   - Download and install [Docker](https://www.docker.com/).

2. **Clone the Repository:**

   ```bash
   git clone https://github.com/yourusername/url-shortener.git
   ```

3. **Navigate to the Project Directory:**

   ```bash
   cd url-shortener
   ```

4. **Set Up PostgreSQL Database:**

   - You can run PostgreSQL using Docker:
     ```bash
     docker run --name urlshortener-db -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin -e POSTGRES_DB=urlshortener -p 5432:5432 -d postgres
     ```
   - Alternatively, install PostgreSQL locally and ensure it is running.

5. **Run Database Migrations:**

   ```bash
   dotnet ef database update
   ```

6. **Build and Run the .NET Backend:**

   ```bash
   dotnet build
   dotnet run
   ```

7. **Run the Application Using Docker Compose:**

   ```bash
   docker compose up --build
   ```

   This will build and start the application in a containerized environment.

8. **Access the Application:**
   Open your browser and navigate to:
   ```
   http://localhost:3000
   ```

---

### Using Online Version

If you prefer not to run the application locally, visit the live version here:
👉 [bytely.xyz](https://bytely.xyz)

---

## Tech Stack

- **Frontend:** CSS
- **Backend:** .NET 7

---

## Contributing

1. Fork the repository.
2. Create a new branch: `git checkout -b feature-name`.
3. Make your changes and commit: `git commit -m "Add feature-name"`.
4. Push to the branch: `git push origin feature-name`.
5. Submit a pull request.

---

## License

No license, just for fun.
---

