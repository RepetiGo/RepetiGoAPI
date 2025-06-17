# RepetiGo API

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![API Status](https://img.shields.io/badge/status-active-brightgreen.svg)]()

**RepetiGo** is a modern, intelligent spaced repetition learning platform that helps users memorize information efficiently using scientifically-proven memory techniques. Built with .NET 9 and featuring AI-powered content generation.

## 🌟 Features

### Core Learning Features

-   **🧠 Advanced Spaced Repetition**: Custom SM-2 algorithm with adaptive scheduling
-   **🤖 AI-Powered Card Generation**: Generate flashcards with Google Gemini AI
-   **📱 Multi-Media Cards**: Support for text and images with Cloudinary integration
-   **📊 Progress Tracking**: Detailed learning analytics and statistics
-   **⚙️ Customizable Settings**: Fine-tune learning parameters to your preference
-   **📅 Due Cards Management**: Smart scheduling for optimal review timing

### User Experience

-   **🔐 Secure Authentication**: JWT-based auth with email verification
-   **👤 User Profiles**: Avatar management and personalization
-   **🌐 Community Sharing**: Share and discover public deck libraries
-   **📱 RESTful API**: Clean, well-documented API for frontend integration
-   **📧 Email Notifications**: Password reset and account verification

### Technical Excellence

-   **🏗️ Clean Architecture**: SOLID principles with repository pattern
-   **🔄 Unit of Work**: Consistent data management
-   **📝 Auto-Mapping**: Efficient DTO transformations with AutoMapper
-   **🛡️ Input Validation**: Comprehensive data validation
-   **📄 API Documentation**: Swagger/OpenAPI integration
-   **🚀 Rate Limiting**: Built-in protection against abuse
-   **💾 Caching**: Redis-based caching for improved performance
-   **🔒 Security**: CORS, authentication, and authorization

## 🧠 Spaced Repetition System

RepetiGo implements a custom SM-2 algorithm for optimal learning:

### Algorithm Properties
- **Easiness Factor (EF)**: Starts at 2.5, adjusts based on performance
- **Repetition Count**: Tracks how many times a card has been reviewed
- **Learning Steps**: Manages the initial learning phase
- **Next Review**: Calculates optimal review timing

### Review Ratings
- **Again (1)**: Card was difficult, repeat sooner
- **Hard (2)**: Card was somewhat difficult
- **Good (3)**: Card was answered correctly
- **Easy (4)**: Card was very easy

### Scheduling Logic
- New cards start in learning phase
- Cards graduate to review phase after successful reviews
- Intervals increase exponentially based on performance
- Failed cards return to learning phase

## 🤖 AI Integration

### Google Gemini Integration
- **Card Generation**: Create flashcards from topics or text
- **Content Enhancement**: Improve existing card content
- **Image Generation**: Generate relevant images for cards
- **Smart Suggestions**: AI-powered study recommendations

## 🛡️ Security Features

### Authentication & Authorization
- JWT-based authentication with refresh tokens
- Role-based access control
- Secure password hashing with ASP.NET Core Identity
- Email verification for new accounts

### Data Protection
- Input validation and sanitization
- SQL injection prevention with Entity Framework
- XSS protection with proper content encoding
- CORS configuration for cross-origin requests

### Rate Limiting
- API rate limiting to prevent abuse
- Separate limits for AI generation endpoints
- Configurable limits per user/IP

## 📊 Database Schema

### Core Tables
- **AspNetUsers**: User accounts and authentication
- **Decks**: Flashcard collections
- **Cards**: Individual flashcards with SRS data
- **Reviews**: Review history and performance tracking
- **Settings**: User preferences and configuration

### Relationships
- Users can have multiple decks
- Decks contain multiple cards
- Cards have multiple reviews
- Each user has one settings record

## 🚀 Deployment

### Production Considerations
- Use production-grade database (Azure SQL, AWS RDS)
- Configure Redis for caching
- Set up proper logging and monitoring
- Use HTTPS in production
- Configure proper CORS origins
- Set up CI/CD pipeline

## 🧪 Testing

### Test Structure
- Unit tests for services and repositories
- Integration tests for API endpoints
- Database tests with in-memory provider

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow C# coding conventions
- Write unit tests for new features
- Update documentation for API changes
- Use conventional commit messages

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Documentation**: [API Documentation](https://localhost:{PORT}/swagger)
- **Issues**: [GitHub Issues](https://github.com/RepetiGo/Server/issues)
- **Discussions**: [GitHub Discussions](https://github.com/RepetiGo/Server/discussions)

## 🙏 Acknowledgments

- **SM-2 Algorithm**: Based on the SuperMemo 2 spaced repetition algorithm
- **Google Gemini**: AI-powered content generation
- **Cloudinary**: Image hosting and manipulation
- **ASP.NET Core**: Modern web framework
- **Entity Framework**: Data access and ORM

---

**RepetiGo** - Making learning efficient and intelligent! 🧠✨