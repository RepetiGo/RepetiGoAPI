# RepetiGo API

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![API Status](https://img.shields.io/badge/status-active-brightgreen.svg)]()

**RepetiGo** is a modern, intelligent spaced repetition learning platform that helps users memorize information efficiently using scientifically-proven memory techniques. Built with .NET 9 and featuring AI-powered content generation.

## 📋 Prerequisites

### Required Software
- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Git** for version control

### Optional Tools
- **Visual Studio 2022** (with .NET desktop development workload) or **Visual Studio Code**
- **SQL Server Management Studio (SSMS)** for database management (if using local SQL Server)
- **Postman** or similar tool for API testing

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone <repository-url>
cd RepetiGo
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Required Files Setup

**File Location**:
* Place the `gen-lang-client-0042585374-cee6dd59fb76.json` file in the `RepetiGo.Api` folder (same directory as `Program.cs`).
* Place the `appsettings.json` file in the same directory as well.
* Place the `launchSettings.json` file in the `Properties` folder of the `RepetiGo.Api` project.

```
RepetiGo/
├── RepetiGo.Api/
│   ├── Properties/launchSettings.json ← Place here
│   ├── gen-lang-client-0042585374-cee6dd59fb76.json  ← Place here
│   ├── Program.cs
│   ├── appsettings.json ← Place here
│   └── ...
```

### 4. Run the Application
```bash
# From the root directory
cd RepetiGo.Api
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5147`
- **Swagger UI**: `http://localhost:5147/swagger` (in Development mode)

## 🧪 Running Tests

### Run All Tests
```bash
# From the root directory
cd RepetiGo.Api.Tests
dotnet test
```

## 📱 Using the API

### Launch Profiles
The project has two launch profiles configured in [`Properties/launchSettings.json`](RepetiGo.Api/Properties/launchSettings.json):

#### HTTP Profile
```bash
dotnet run --launch-profile http
```
- URL: `http://localhost:5147`
- Environment: Development

### API Documentation
- **Swagger UI**: `http://localhost:5147/swagger` (Development only)
- **OpenAPI Spec**: `http://localhost:5147/openapi/v1.json`

## 🐛 Troubleshooting

### Common Issues

#### 1. SSL Certificate Issues
```bash
# Trust the development certificate
dotnet dev-certs https --trust
```

#### 2. Port Already in Use
- The application uses ports 5147 (HTTP) and 7213 (HTTPS)
- Kill any processes using these ports if needed
- Ports can be changed in [`Properties/launchSettings.json`](RepetiGo.Api/Properties/launchSettings.json)

#### 3. Package Restore Issues
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore --force
```

## 🏗️ Project Structure

### Main Projects
- **[`RepetiGo.Api`](RepetiGo.Api)**: Main API project
- **[`RepetiGo.Api.Tests`](RepetiGo.Api.Tests)**: Unit and integration tests

### Key Components
- **Controllers**: API endpoints in [`RepetiGo.Api/Controllers`](RepetiGo.Api/Controllers)
- **Services**: Business logic in [`RepetiGo.Api/Services`](RepetiGo.Api/Services)
- **Models**: Data models in [`RepetiGo.Api/Models`](RepetiGo.Api/Models)
- **Data**: Database context and migrations in [`RepetiGo.Api/Data`](RepetiGo.Api/Data)

### Features Available
- ✅ **User Registration & Authentication**
- ✅ **Email Verification**
- ✅ **Password Reset**
- ✅ **Deck Management**
- ✅ **Flashcard Creation & Management**
- ✅ **AI-Powered Card Generation** (Google Gemini)
- ✅ **Image Upload** (Cloudinary)
- ✅ **Spaced Repetition Algorithm** (SM-2)
- ✅ **Review System**
- ✅ **User Settings**
- ✅ **Rate Limiting**
- ✅ **Caching** (Redis)

## 🧠 Spaced Repetition System

The application implements a custom SM-2 algorithm with the following default settings:
- **Learning Steps**: "25m 1d" (25 minutes, then 1 day)
- **Graduating Interval**: 3 days
- **Easy Interval**: 4 days
- **Starting Easiness Factor**: 2.5

## 📊 Database Schema

### Core Tables
- **AspNetUsers**: User accounts and authentication
- **Decks**: Flashcard collections
- **Cards**: Individual flashcards with SRS data
- **Reviews**: Review history and performance tracking
- **Settings**: User preferences and SRS configuration

## 📞 Support

If you encounter any issues:
1. Check the troubleshooting section above
2. Review the application logs in the console
3. Ensure all prerequisites are installed
4. Verify the application is running on the correct ports

## 🔍 Testing the Application

### Quick Test Steps
1. Start the application: `dotnet run`
2. Open browser to: `http://localhost:5147/swagger`
3. Try the registration endpoint with sample data
4. Check email verification (logs will show the verification link)
5. Test login with verified account
6. Create a deck and add cards
7. Test the AI card generation features

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

**The application is fully configured and ready to run!** 🚀