# Pizza Sales Dashboard

A full-stack web application built with **Angular** and **ASP.NET Core** that visualizes pizza shop sales data using interactive charts and statistical reports.

This project imports a real-world pizza sales dataset from Kaggle and presents business insights through a REST API and frontend dashboard.

---

## Tech Stack

- **Frontend**: Angular 16+, ng-apexcharts
- **Backend**: ASP.NET Core Web API (.NET 8), Entity Framework Core
- **Database**: SQL Server (Express or LocalDB)
- **Charting**: ApexCharts

---

## Features

### Import + Store Data
- Import CSV pizza sales data into a SQL Server database schema.

### RESTful API Endpoints
- `/api/Statistics/bestsellers` — Top 5 best-selling pizzas
- `/api/Statistics/peak-hours` — Orders by hour (00:00 to 23:00)
- `/api/Statistics/daily-orders` — Orders grouped by day (last 10 days of data)
- `/api/Statistics/underperformers` — Pizzas with low or zero sales

and others.

### Dashboard
- Interactive bar, line, and donut charts using `ng-apexcharts`
- Dynamic time formatting (e.g., `03:00 PM`)
- Clean, responsive layout
