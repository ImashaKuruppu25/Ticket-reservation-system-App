# Ticket-reservation-system-App
![image](https://github.com/ImashaKuruppu25/Ticket-reservation-system-App/assets/79103952/b61d0abf-0038-45fb-a935-3b4c10b211aa)


🎨 Figma File = https://www.figma.com/file/lRfXCSdaRkipHYscJJCbkn/UI-designs?type=design&node-id=0-1&mode=design

📄 Swagger api doc = https://ticket-reservation-system20231007002452.azurewebsites.net/swagger/index.html

## Few UIs
![image](https://github.com/ImashaKuruppu25/Ticket-reservation-system-App/assets/79103952/c451037a-3352-41bd-9915-115330abb3d3)
![image](https://github.com/ImashaKuruppu25/Ticket-reservation-system-App/assets/79103952/ee749d1a-5ac8-4024-8c70-640fdd772008)
![image](https://github.com/ImashaKuruppu25/Ticket-reservation-system-App/assets/79103952/8735bbcc-394a-40db-b797-70ee90131bc0)
![image](https://github.com/ImashaKuruppu25/Ticket-reservation-system-App/assets/79103952/b7264b46-90cf-4006-9ecf-3340ee1909c5)
![image](https://github.com/ImashaKuruppu25/Ticket-reservation-system-App/assets/79103952/d9f5fd2b-0812-4fbe-a91a-1e96bff625f1)
![image](https://github.com/ImashaKuruppu25/Ticket-reservation-system-App/assets/79103952/03dae200-c0ed-4b88-ac24-f8a63b86282a)
![image](https://github.com/ImashaKuruppu25/Ticket-reservation-system-App/assets/79103952/4850a329-0326-4a9e-8baf-57ec784ba610)




## Introduction
Welcome to the Ticket Reservation System! This application allows you to efficiently manage users, travelers, ticket bookings, and train schedules. Below, we outline the key functionalities of the system.

## User Roles
The application has two distinct user roles:

- **Backoffice**: Users with this role have access to advanced functions.
- **Travel Agent**: Users with this role have limited access compared to Backoffice users.

## Traveler Management
Traveler Management allows you to create, update, delete, activate, and deactivate traveler profiles using their National Identification Card (NIC) as the primary key. Here are the key functionalities:

- [x] Create new traveler profiles
- [x] Update existing traveler profiles
- [x] Delete traveler profiles
- [x] Activate and deactivate traveler accounts

## Ticket Booking Management
With Ticket Booking Management, you can manage ticket reservations efficiently. Key features include:

- [x] Creating new reservations (reservation date within 30 days from the booking date, maximum 4 reservations per reference ID).
- [x] Updating reservations (at least 5 days before the reservation date).
- [x] Canceling reservations (at least 5 days before the reservation date).

## Train Management
Train Management allows you to handle train details and schedules effectively:

- [x] Creating new trains with schedules (trains can only be reserved once they are active and published).
- [x] Updating existing train schedules.
- [x] Canceling trains for reservations (cannot cancel a train with existing reservations).

## Getting Started
To get started with the Ticket Reservation System, follow these steps:

### Prerequisites
Before using the application, make sure you have the following prerequisites installed:
- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Database system : MongoDB]
- Dependencies specified in the project's programe.cs file

### Installation
1. Clone the repository:
   ```shell
   git clone https://github.com/ImashaKuruppu25/Ticket-reservation-system-App.git


![image](https://github.com/ImashaKuruppu25/Ticket-reservation-system-App/assets/79103952/70514981-8b92-4748-af78-91416e3abd2e)
