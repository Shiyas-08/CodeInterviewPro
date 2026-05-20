# CodeInterviewPro

CodeInterviewPro is a cutting-edge, AI-driven technical interview platform designed to facilitate seamless, real-time coding assessments. It empowers interviewers and candidates to collaborate in real-time with live code synchronization, integrated video/audio communication, and an advanced, secure multi-language code execution engine.

To ensure the highest quality of evaluation, CodeInterviewPro employs a **Dual-AI Evaluation System** utilizing **Google Gemini** and **CodeBert** to provide instant, human-like feedback, complexity scoring, and plagiarism detection.

---

## ✨ Key Features

- **Real-Time Code Collaboration:** Built with WebSockets (SignalR) to instantly sync code editor typing between the candidate and the interviewer.
- **Integrated Video & Audio:** Peer-to-peer WebRTC integration enables live face-to-face communication without requiring third-party tools.
- **Secure Code Execution Pipeline:** Safely compiles and executes user-submitted code in multiple languages (C#, Python, Java, Go, JS) using sandboxed, ephemeral Docker containers.
- **Dual-AI Evaluation:**
  - **CodeBert:** Performs deep AST (Abstract Syntax Tree) analysis, calculates cyclomatic complexity, and checks code similarity (plagiarism detection).
  - **Google Gemini:** Acts as a virtual senior interviewer, taking the code, CodeBert's metrics, and test case results to generate a human-readable performance report and hiring recommendation.
- **Asynchronous AI Processing:** Uses an in-memory Background Task Queue to process heavy AI requests without blocking the user interface.
- **High Performance & Rate Limiting:** Utilizes Redis for caching code execution results and API rate limiting to ensure optimal performance under load.

---

## 🛠️ Technology Stack

### Backend
- **Framework:** .NET 8 (C#) adhering to Clean Architecture principles
- **Database:** Microsoft SQL Server with Entity Framework Core
- **Caching & Rate Limiting:** Redis
- **Real-time Comms:** SignalR (WebSockets)
- **Code Execution:** Docker Engine Integration via `/var/run/docker.sock`

### Frontend
- **Framework:** Angular (Standalone Components architecture)
- **Styling:** Tailwind CSS & Custom CSS
- **Code Editor:** Monaco Editor
- **Video/Audio:** WebRTC browser APIs

### AI & Machine Learning
- **Generative AI:** Google Gemini (`gemini-3-flash-preview` API)
- **Code Analysis Model:** CodeBert (Hosted locally via a Python microservice)

### DevOps & Testing
- **Containerization:** Docker & Docker Compose
- **Stress Testing:** k6 (Performance and load testing for the code execution pipeline)

---

## 🏗️ Architecture Overview

The system is designed with **Clean Architecture** for maximum separation of concerns:
1. **Domain Layer:** Contains core entities (User, InterviewSession, ExecutionResult) and enums.
2. **Application Layer:** Contains DTOs, interfaces, and core business logic.
3. **Infrastructure Layer:** Handles all external concerns: Entity Framework DbContext, Redis caching, Docker code execution integration, and external AI service clients (Gemini & CodeBert).
4. **API Layer:** ASP.NET Core REST API exposing secure endpoints.

### Code Execution Security
Executing untrusted code is inherently risky. CodeInterviewPro handles this by:
1. Receiving code at the API level.
2. Communicating with the host's Docker daemon.
3. Spinning up a temporary `code-runner` container with network isolation.
4. Enforcing strict limits on memory (e.g., 256MB) and execution time (e.g., 30 seconds).
5. Destroying the container immediately after execution.

---

## 🚀 Getting Started

### Prerequisites
- Docker & Docker Compose
- .NET 8 SDK
- Node.js & npm (for Angular frontend)
- API Keys for Google Gemini

### Local Setup (Using Docker Compose)

The easiest way to run the entire backend stack (API, Redis, SQL Server, and AI microservices) is using Docker Compose.

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/CodeInterviewPro.git
   cd CodeInterviewPro
   ```

2. **Configure Environment Variables:**
   Update the `docker-compose.yml` or your local `.env` file with your Gemini API key:
   ```env
   GEMINI_API_KEY=your_google_gemini_api_key_here
   ```

3. **Build the Runner Image:**
   The API needs the `code-runner` image to execute code. Build it first:
   ```bash
   docker build -t code-runner -f Dockerfile.runner .
   ```

4. **Spin up the backend services:**
   ```bash
   docker-compose up --build -d
   ```
   *This will start the .NET API (port 7000), SQL Server, Redis, and ensure the host's Docker socket is mounted for code execution.*

5. **Start the Frontend:**
   ```bash
   cd codeinterviewpro-ui
   npm install
   npm start
   ```
   *The Angular application will be available at `http://localhost:4200`.*

---

## 🧪 Testing

The repository includes a comprehensive `k6` stress testing suite located in the `/tests/k6` directory.
To run a stress test against the code execution pipeline:
```bash
k6 run tests/k6/scripts/stress_code_execution.js
```

---

## 📄 License

This project is proprietary and built for demonstration, technical evaluation, and portfolio purposes.
