# Platform - Safarny ✈️

## 📚 Table of Contents
- [Overview](#-overview)
- [Background](#-background)
- [Features](#-features)
  - [User Registration and Login](#user-registration-and-login)
  - [Tourist Features](#tourist-features)
  - [Admin Features](#admin-features)
  - [Payment Integration](#payment-integration)
- [Database Schema](#️-database-schema)
- [UML Diagram](#uml-diagram)
- [Requirements Specification](#requirements-specification)
- [Flow Charts](#flow-charts)

---

## 🧭 Overview
Welcome to **Safarny** a smart tourism platform that helps travelers explore Egypt with ease, efficiency, and intelligence. Whether you're a solo tourist, a couple, or a family, Safarny connects you with the best travel experiences, hotels, restaurants, and historical sites across Egypt. This document presents an overview of the project, its purpose, and how to interact with it.

---

## 🌍 Background
In today's fast-paced world, travelers seek personalized experiences, quick access to services, and hassle-free planning. However, tourism platforms often lack smart features or localized insight. **Safarny** solves these challenges by:
- Offering tailored trip suggestions based on user preferences.
- Helping users discover hidden gems in Egypt through categorized browsing.
- Enabling full trip planning and payment – all in one place.

With AI-powered assistance and rich content, Safarny transforms travel planning into a smooth, enjoyable journey.

---

## ✨ Features

### 🔐 User Registration and Login
- Tourists and Admins can register accounts.
- Secure authentication with role-based access (via ASP.NET Core Identity).

### 🧳 Tourist Features
- **Smart Onboarding**:
  - Answer 3 key questions: Budget, Trip Type, Number of Days in Egypt.
  - Get personalized package suggestions based on answers.

- **Home Page Categories**:
  - Ancient Egypt, Beaches, Cities, Sports.

- **City-Based Navigation**:
  - View cities like Cairo, Alexandria, etc.
  - For each city, explore:
    - 🏨 Hotels: features, images, rooms, booking, reviews.
    - 🍽️ Restaurants: filtered by cuisine (e.g., Chinese, Breakfast).
    - 📍 Places: with activities and descriptions.

- **Packages Section**:
  - Browse ready-made packages with:
    - Day-by-day itinerary
    - What’s included/excluded
    - Ratings and reviews
    - Booking for multiple persons + online payment

- **Plan Your Trip**:
  - Choose cities, activities, and hotels.
  - View real-time trip plan, total cost, number of days.
  - Book and pay online.

- **AI Chatbot**:
  - Ask anything like:
    - "How do I go from Cairo to Luxor?"
    - "What’s the best place for a family in Alexandria?"
  - Powered by DeepSeek / OpenAI APIs.

### 🛠️ Admin Features
- Full CRUD access to:
  - Cities
  - Hotels
  - Restaurants
  - Tourist places
  - Packages
- Approve user content.
- Track system usage and manage listings.

### 💳 Payment Integration
- Online payment system for:
  - Hotels
  - Packages
  - Custom trips
- Integrated with Paymob or any third-party payment gateway.

## 🗄️ Database Schema

The Safarny application uses a relational database to organize data for users, cities, hotels, restaurants, places, packages, custom trips, bookings, and chatbot queries.

Here is a summary of the main tables with primary keys (PK) and foreign key relationships (FK):

## 🗄️ Database Schema

The Safarny application uses a relational database to manage users, cities, hotels, restaurants, places, packages, trips, bookings, and chatbot queries.

```yaml
users:
  - userId: PK
  - username
  - email
  - passwordHash
  - role  # e.g., Tourist, Admin
  - phoneNumber
  - createdAt
  - updatedAt

cities:
  - cityId: PK
  - cityName
  - description
  - category  # Ancient Egypt, Beaches, Cities, Sports
  - imageUrl

hotels:
  - hotelId: PK
  - cityId: FK references cities.cityId
  - hotelName
  - description
  - features  # e.g., view, wifi, pool
  - rating
  - mainImageUrl
  - createdAt
  - updatedAt

hotelRooms:
  - roomId: PK
  - hotelId: FK references hotels.hotelId
  - roomType  # e.g., single, double, suite
  - pricePerNight
  - availabilityCount

restaurants:
  - restaurantId: PK
  - cityId: FK references cities.cityId
  - restaurantName
  - cuisineType  # e.g., Chinese, Breakfast, Egyptian
  - description
  - rating
  - mainImageUrl
  - createdAt
  - updatedAt

places:
  - placeId: PK
  - cityId: FK references cities.cityId
  - placeName
  - description
  - activities  # text or linked table
  - mainImageUrl
  - rating
  - createdAt
  - updatedAt

packages:
  - packageId: PK
  - packageName
  - description
  - durationDays
  - price
  - itineraryDetails  # day by day plan
  - includedServices
  - excludedServices
  - averageRating
  - createdAt
  - updatedAt

packageReviews:
  - reviewId: PK
  - packageId: FK references packages.packageId
  - userId: FK references users.userId
  - rating
  - comment
  - createdAt

customTrips:
  - tripId: PK
  - userId: FK references users.userId
  - startDate
  - endDate
  - totalPrice
  - tripDetails  # JSON or detailed plan
  - createdAt
  - updatedAt

bookings:
  - bookingId: PK
  - userId: FK references users.userId
  - bookingType  # e.g., hotel, package, customTrip
  - referenceId  # FK depending on bookingType, e.g., hotelRoomId, packageId, tripId
  - numberOfPeople
  - bookingDate
  - totalPrice
  - paymentStatus
  - createdAt
  - updatedAt

chatbotQueries:
  - queryId: PK
  - userId: FK references users.userId
  - question
  - response
  - queryDate
```
## UML Diagram

The UML diagram illustrates the relationships between the core entities of the Safarny smart tourism platform.

```yaml
+--------------------------------------+
|             User                     |
+--------------------------------------+
| userId: Long                         |
| username: String                     |
| email: String                       |
| passwordHash: String                 |
| role: Role                          |
| phoneNumber: String                 |
| createdAt: DateTime                 |
| updatedAt: DateTime                 |
| bookings: List<Booking>             |
| customTrips: List<CustomTrip>       |
| packageReviews: List<PackageReview> |
| chatbotQueries: List<ChatbotQuery>  |
+--------------------------------------+

+--------------------------------------+
|             City                     |
+--------------------------------------+
| cityId: Long                        |
| cityName: String                   |
| description: String                 |
| category: String                   |
| imageUrl: String                   |
| hotels: List<Hotel>                |
| restaurants: List<Restaurant>       |
| places: List<Place>                 |
+--------------------------------------+

+--------------------------------------+
|             Hotel                    |
+--------------------------------------+
| hotelId: Long                      |
| cityId: Long                      |
| hotelName: String                  |
| description: String                |
| features: String                  |
| rating: Float                     |
| mainImageUrl: String              |
| createdAt: DateTime               |
| updatedAt: DateTime               |
| rooms: List<HotelRoom>             |
+--------------------------------------+

+--------------------------------------+
|           HotelRoom                  |
+--------------------------------------+
| roomId: Long                      |
| hotelId: Long                    |
| roomType: String                 |
| pricePerNight: Decimal           |
| availabilityCount: Int           |
+--------------------------------------+

+--------------------------------------+
|          Restaurant                  |
+--------------------------------------+
| restaurantId: Long                |
| cityId: Long                    |
| restaurantName: String           |
| cuisineType: String              |
| description: String              |
| rating: Float                   |
| mainImageUrl: String            |
| createdAt: DateTime             |
| updatedAt: DateTime             |
+--------------------------------------+

+--------------------------------------+
|             Place                   |
+--------------------------------------+
| placeId: Long                    |
| cityId: Long                    |
| placeName: String                |
| description: String              |
| activities: String or List<String> |
| mainImageUrl: String            |
| rating: Float                   |
| createdAt: DateTime             |
| updatedAt: DateTime             |
+--------------------------------------+

+--------------------------------------+
|            Package                  |
+--------------------------------------+
| packageId: Long                  |
| packageName: String              |
| description: String              |
| durationDays: Int                |
| price: Decimal                  |
| itineraryDetails: String         |
| includedServices: String         |
| excludedServices: String         |
| averageRating: Float             |
| createdAt: DateTime             |
| updatedAt: DateTime             |
| reviews: List<PackageReview>     |
+--------------------------------------+

+--------------------------------------+
|         PackageReview               |
+--------------------------------------+
| reviewId: Long                   |
| packageId: Long                 |
| userId: Long                    |
| rating: Int                    |
| comment: String                |
| createdAt: DateTime            |
+--------------------------------------+

+--------------------------------------+
|          CustomTrip                 |
+--------------------------------------+
| tripId: Long                    |
| userId: Long                   |
| startDate: DateTime            |
| endDate: DateTime              |
| totalPrice: Decimal            |
| tripDetails: String (JSON)     |
| createdAt: DateTime            |
| updatedAt: DateTime            |
+--------------------------------------+

+--------------------------------------+
|            Booking                  |
+--------------------------------------+
| bookingId: Long                 |
| userId: Long                   |
| bookingType: String            |
| referenceId: Long              |
| numberOfPeople: Int            |
| bookingDate: DateTime          |
| totalPrice: Decimal            |
| paymentStatus: String          |
| createdAt: DateTime            |
| updatedAt: DateTime            |
+--------------------------------------+

+--------------------------------------+
|          ChatbotQuery               |
+--------------------------------------+
| queryId: Long                   |
| userId: Long                   |
| question: String               |
| response: String               |
| queryDate: DateTime            |
+--------------------------------------+
```
## Requirements Specification

Software Requirements Specification (SRS) is the process of documenting all system and user requirements. Below is a detailed explanation of the functional and non-functional requirements of the **Safarny** smart tourism platform.

### Functional Requirements

These define what the system does, highlighting the features and functions the application offers.

#### User Registration and Login
- Tourists and Admins can create new accounts and log in.
- Secure authentication and role-based authorization to control access to different parts of the platform.

#### Tourist Features
- **Smart Onboarding:**  
  Users answer three key questions (budget, trip type, and duration of stay) to receive personalized travel package suggestions.

- **Browse Categories:**  
  Tourists can explore categories such as Ancient Egypt, Beaches, Cities, and Sports.

- **City Exploration:**  
  Users can select a city (e.g., Cairo, Alexandria) and explore:  
  - Hotels (with features, images, room availability, booking, and payment).  
  - Restaurants (with cuisine type filters like Chinese, Breakfast).  
  - Tourist Places (with activities and descriptions).

- **Packages Section:**  
  Users can browse ready-made travel packages including:  
  - Detailed day-by-day itineraries.  
  - Information about included and excluded services.  
  - Ratings and reviews.  
  - Booking for multiple persons with online payment.

- **Plan Your Trip:**  
  Allows users to customize trips by selecting cities, activities, and hotels.  
  - Displays total cost, trip duration, and schedule.  
  - Enables booking and payment online.

- **AI Chatbot:**  
  Provides instant answers to travel-related questions (e.g., directions between cities, best family-friendly spots) powered by AI integration.

#### Admin Features
- Manage content including cities, hotels, restaurants, tourist places, and packages.  
- Approve user-generated content and monitor platform usage.

#### Booking and Payment
- Support online payments for hotels, packages, and custom trips through integrated payment gateways.

---

### Non-Functional Requirements

These specify the operational capabilities and constraints of the system.

- **Performance:**  
  The platform should respond to user interactions and page loads within 3 seconds under normal network conditions.

- **Security:**  
  User data, including login credentials and payment information, is securely encrypted. Authentication uses JWT tokens and best practices in password storage.

- **Availability:**  
  The system should maintain high availability (aiming for 99.9% uptime), with planned maintenance during low-traffic periods.

- **Scalability:**  
  The platform must support increasing numbers of users and data without significant performance degradation.
## Flow Charts

The Flow Charts provide a visual representation of the main user journeys and processes within the Safarny platform. These diagrams help to understand how users interact with the system through different workflows.

### Login Flow
This flow chart illustrates the step-by-step process for user authentication, including registration, login, and role-based access control for tourists and admins.
![Login Flow](https://github.com/user-attachments/assets/297b8232-4cb6-4e06-8f46-f6dd88573a6d)

### Trip Builder Flow
This flow chart shows how users create and customize their own trips by selecting cities, activities, hotels, and how the system calculates the total cost and trip duration before final booking.
Refer to the Flow Charts section or attached diagram files for detailed visualizations of these processes.
![Trip Builder Flow](https://github.com/user-attachments/assets/fefe26fd-36d1-418d-a392-827bfa4b3f07)
