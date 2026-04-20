🔐 ASP.NET Core Identity & JWT Project

Bu proje, ASP.NET Core ile geliştirilmiş bir kimlik doğrulama ve yetkilendirme sistemidir.
Hem Cookie Authentication hem de JWT yapıları birlikte kullanılarak gerçek dünya senaryoları uygulanmıştır.

🚀 Özellikler
Register / Login / Logout işlemleri
Email doğrulama & şifre sıfırlama
Role, Claim ve Policy tabanlı yetkilendirme
JWT ile güvenli API erişimi
Kullanıcı profil yönetimi
Inbox / Outbox mesajlaşma sistemi
🛠️ Kullanılan Teknolojiler
ASP.NET Core
Entity Framework Core
ASP.NET Core Identity
JWT (JSON Web Token)
FluentValidation
AutoMapper

🧩 Mimari

Katmanlı mimari kullanılmıştır:

UI (Presentation)
Business (Service)
Data Access (DAL)
Entity

🔄 AutoMapper

Katmanlar arası veri dönüşümünü kolaylaştırır.
Kod tekrarını azaltır ve daha temiz bir yapı sağlar.

✅ FluentValidation

Kullanıcıdan gelen verilerin doğrulanmasını sağlar.
Controller’ları sade tutar ve merkezi validation yönetimi sunar.

🎯 Amaç

Bu proje, ASP.NET Core ile güvenli ve ölçeklenebilir bir authentication sistemi geliştirmeyi öğrenmek amacıyla hazırlanmıştır.
