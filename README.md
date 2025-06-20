Platform - Safarny ✈️

📚 Table of Contents

Overview

Background

Features

User Registration and Login

Tourist Features

Admin Features

Payment Integration

Database Schema

UML Diagram

Requirements Specification

Use Case Diagram

ERD Diagram

🧭 Overview

Safarny is a smart tourism platform that connects local and international travelers with the best destinations, hotels, restaurants, and activities across Egypt. Powered by intelligent search and personalized trip planning, it simplifies your entire travel experience — from exploration to booking.

🌍 Background

Planning a trip to Egypt can be difficult due to scattered information, lack of personalization, and language barriers. Safarny addresses these issues by offering:

A unified experience that understands your preferences.

Smart suggestions using AI.

Full trip customization, hotel booking, and payment — all in one place.

Whether you're exploring ancient ruins, relaxing on the beach, or discovering vibrant cities, Safarny is your perfect travel partner.

✨ Features

🔐 User Registration and Login

Tourists and Admins can create accounts securely.

ASP.NET Core Identity with role-based access (Tourist, Admin).

🧳 Tourist Features

Smart Onboarding:

Answer 3 key questions: Budget, Trip Type, Number of Days in Egypt.

Get personalized package suggestions based on answers.

Home Page Categories:

Ancient Egypt, Beaches, Cities, Sports.

City-Based Navigation:

View cities like Cairo, Alexandria, etc.

For each city, explore:

🏨 Hotels: features, images, rooms, booking, reviews.

🍽️ Restaurants: filtered by cuisine (e.g., Chinese, Breakfast).

📍 Places: with activities and descriptions.

Packages Section:

Browse ready-made packages with:

Day-by-day itinerary

What’s included/excluded

Ratings and reviews

Booking for multiple persons + online payment

Plan Your Trip:

Choose cities, activities, and hotels.

View real-time trip plan, total cost, number of days.

Book and pay online.

AI Chatbot:

Ask anything like:

"How do I go from Cairo to Luxor?"

"What’s the best place for a family in Alexandria?"

Powered by DeepSeek / OpenAI APIs.

🛠️ Admin Features

Full CRUD access to:

Cities

Hotels

Restaurants

Tourist places

Packages

Approve user content.

Track system usage and manage listings.

💳 Payment Integration

Online payment system for:

Hotels

Packages

Custom trips

Integrated with Paymob or any third-party payment gateway
