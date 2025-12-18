# 🏋️‍♂️ Fitness Center Yönetim Sistemi

Bu proje, Web Programlama dersi kapsamında geliştirilmiş bir **Spor Salonu Yönetim ve Randevu Sistemi**dir.
ASP.NET Core 8 MVC teknolojisi kullanılarak, modern web standartlarına uygun olarak tasarlanmıştır.

## 🚀 Proje Özellikleri

Proje, hem yönetim (Admin) hem de kullanıcı (Üye) tarafında kapsamlı özellikler sunar:

### 👤 Kullanıcı (Üye) Modülü
* **Üyelik Sistemi:** Kayıt Ol (Register), Giriş Yap (Login) ve Çıkış (Logout) işlemleri.
* **Randevu Alma:** Eğitmen ve hizmete göre müsait saatleri sorgulama ve randevu oluşturma.
* **Randevularım:** Kişisel randevu geçmişini görüntüleme ve iptal edebilme.
* **🤖 AI Antrenör:** Google Gemini Yapay Zeka destekli kişisel egzersiz ve beslenme tavsiyesi alma.

### 🛡️ Yönetim (Admin) Paneli
* **Hizmet Yönetimi:** Spor salonu hizmetlerini (Pilates, Yoga vb.) ekleme, düzenleme, silme.
* **Eğitmen Yönetimi:** Antrenör ekleme, resim yükleme ve verilen dersleri (Checkbox ile) atama.
* **Müsaitlik Yönetimi:** Eğitmenlerin çalışma gün ve saatlerini (Slot) belirleme.
* **Randevu Yönetimi:** Tüm randevuları görüntüleme, onaylama (Approve) veya silme yetkisi.
* **Kullanıcı Yönetimi:** Kayıtlı üyeleri listeleme ve gereksiz hesapları silme.

## 🛠️ Kullanılan Teknolojiler

* **Framework:** ASP.NET Core 8.0 MVC
* **Veritabanı:** Microsoft SQL Server (LocalDB)
* **ORM:** Entity Framework Core (Code First Yaklaşımı)
* **Güvenlik:** ASP.NET Core Identity (Kullanıcı ve Rol Yönetimi)
* **Önyüz:** HTML5, CSS3, Bootstrap 5, JavaScript (jQuery)
* **Yapay Zeka:** Google Gemini API Entegrasyonu

## ⚙️ Kurulum ve Çalıştırma

Projeyi kendi bilgisayarınızda çalıştırmak için şu adımları izleyin:

1.  Projeyi klonlayın:
    ```bash
    git clone [https://github.com/KULLANICI_ADIN/FitnessCenterProject.git](https://github.com/KULLANICI_ADIN/FitnessCenterProject.git)
    ```
2.  `appsettings.json` dosyasındaki veritabanı bağlantı dizesini (Connection String) kontrol edin.
3.  **Package Manager Console**'u açın ve veritabanını oluşturun:
    ```powershell
    update-database
    ```
4.  Projeyi çalıştırın.

### 🔑 Yapay Zeka (Gemini) Ayarı
Güvenlik sebebiyle API anahtarı GitHub'a yüklenmemiştir. Yapay zeka modülünü çalıştırmak için:
* `GeminiApiKey` değerini `User Secrets` (Kullanıcı Sırları) üzerinden veya `appsettings.json` dosyasına ekleyerek tanımlayınız.

## 📡 API Kullanımı

Proje, eğitmen verilerini dış dünyaya sunan bir **REST API** içerir.

* **Tüm Eğitmenleri Listeleme:**
  `GET /api/TrainersApi`
  
* **Uzmanlık/Ders Filtreleme:**
  `GET /api/TrainersApi?uzmanlik=Pilates` (Pilates dersi verenleri getirir)

## 🔐 Giriş Bilgileri (Varsayılan)

Proje ilk çalıştığında otomatik olarak Admin hesabı oluşturur (Seed Data).

* **Admin Email:** `g201210029@sakarya.edu.tr`
* **Şifre:** `sau`

---
*Web Programlama Dersi Proje Ödevidir.*

 
