# מערכת ניהול מכירות - Sales Management System

מערכת ניהול מכירות מלאה הבנויה ב-ASP.NET MVC עם תמיכה בעברית (RTL).

## סטאק טכנולוגי

- **שפה/פריימוורק**: ASP.NET MVC (.NET 8)
- **מסד נתונים**: MySQL
- **גישה ל-DB**: Dapper (micro-ORM)
- **תצוגה**: Razor Views (.cshtml)
- **ניהול Sessions**: HttpContext.Session
- **ממשק**: עברית (RTL) עם Bootstrap 5

## תכונות

- **מערכת הרשאות**: Admin ו-Sales עם AuthFilter
- **דשבורד**: תפריט דינמי לפי תפקיד
- **ניהול עובדים**: CRUD מלא (Admin בלבד)
- **ניהול ספקים**: CRUD מלא (Admin בלבד)
- **ניהול מוצרים**: CRUD מלא עם קשר לספקים (Admin בלבד)
- **ניהול מלאי**: CRUD מלא עם התראות מלאי נמוך (Admin בלבד)
- **ניהול לקוחות**: CRUD מלא (Admin + Sales)
- **ניהול הזמנות**: CRUD מלא עם סטטוסים (Admin + Sales)
- **ניהול מכירות**: CRUD מלא עם אמצעי תשלום (Admin + Sales)
- **חיפוש**: בכל טבלה
- **עיצוב**: RTL עם Bootstrap 5

## התקנה והפעלה

### דרישות מקדימות
- .NET 8 SDK
- MySQL Server

### שלב 1: הקמת מסד הנתונים
```bash
mysql -u root -p < Scripts/init.sql
```

### שלב 2: עדכון Connection String
ערוך את `appsettings.json` עם פרטי החיבור שלך:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=sales_db;User=root;Password=YOUR_PASSWORD;CharSet=utf8mb4;"
  }
}
```

### שלב 3: הפעלת המערכת
```bash
dotnet run
```

### התחברות ראשונה
- **שם משתמש**: admin
- **סיסמה**: Admin123

## מבנה הפרויקט

```
SalesManagement/
├── Controllers/          ← בקרים (9 controllers)
├── Models/               ← מודלים (8 models)
├── Views/                ← תצוגות Razor
│   ├── Shared/           ← Layout, ViewImports
│   ├── Account/          ← Login
│   ├── Dashboard/        ← דשבורד
│   ├── Employees/        ← CRUD עובדים
│   ├── Suppliers/        ← CRUD ספקים
│   ├── Products/         ← CRUD מוצרים
│   ├── Inventory/        ← CRUD מלאי
│   ├── Customers/        ← CRUD לקוחות
│   ├── Orders/           ← CRUD הזמנות
│   └── Sales/            ← CRUD מכירות
├── Helpers/              ← AuthFilter
├── Data/                 ← DbConnectionFactory
├── Scripts/              ← סקריפט SQL
├── appsettings.json      ← הגדרות
└── Program.cs            ← נקודת כניסה
```

## הרשאות

| ישות | Admin | Sales |
|------|-------|-------|
| עובדים | CRUD | - |
| ספקים | CRUD | - |
| מוצרים | CRUD | - |
| מלאי | CRUD | - |
| לקוחות | CRUD | CRUD |
| הזמנות | CRUD | CRUD |
| מכירות | CRUD | CRUD |
