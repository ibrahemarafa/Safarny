# Platform - Safarny ✈️

## 📚 Table of Contents
- [Overview](#overview)
- [Background](#background)
- [Features](#features)
  - [User Registration and Login](#user-registration-and-login)
  - [Tourist Features](#tourist-features)
  - [Admin Features](#admin-features)
  - [Payment Integration](#payment-integration)
- [Database Schema](#database-schema)
- [UML Diagram](#uml-diagram)
- [Requirements Specification](#requirements-specification)
- [Use Case Diagram](#use-case-diagram)
- [ERD Diagram](#erd-diagram)

---

## 🧭 Overview
Welcome to **Safarny** – a smart tourism platform that helps travelers explore Egypt with ease, efficiency, and intelligence. Whether you're a solo tourist, a couple, or a family, Safarny connects you with the best travel experiences, hotels, restaurants, and historical sites across Egypt. This document presents an overview of the project, its purpose, and how to interact with it.

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

---
