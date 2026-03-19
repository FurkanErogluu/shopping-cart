# Shopping Cart API (Collaborative Project)

Bu proje, kullanıcıların kişisel veya ortaklaşa alışveriş listeleri oluşturmasına, yönetmesine ve diğer kullanıcılarla etkileşim kurmasına olanak tanıyan kapsamlı bir **Backend API** çözümüdür. Bir ekip projesi olarak geliştirilmiştir.

## Öne Çıkan Özellikler

* **Ekip Çalışması & Git Yönetimi:** Proje, ekip üyelerinin her birinin kendi branchi üzerinden çalıştığı, düzenli kod incelemeleri ve birleştirme süreçlerinin olduğu bir projedir.
* **Gelişmiş Kimlik Doğrulama:** `JWT mekanizması ile güvenli ve sürdürülebilir oturum yönetimi.
* **Sosyal Liste Deneyimi:** Kullanıcılar arası bağlantı (`UserConnection`) kurma, diğer kullanıcıları takip etme ve ortak liste yönetimi gibi eklentiler vardır.
* **Esnek Ürün Yönetimi:** Ürünlerin adet veya ağırlık (kg) birimlerine göre takibi ve "IsChecked" özelliği ile anlık liste tamamlama kontrolü eklenmiştir.


## Veri Modeli ve İlişkiler
Uygulama temel olarak şu varlıklar üzerine kuruludur:
* **User:** Kimlik doğrulama ve profil yönetimi.
* **ShoppingList:** Başlık, açıklama ve durum (tamamlandı/devam ediyor) bilgisi.
* **ShoppingListMember:** Bir listeye birden fazla kullanıcının dahil olabilmesi (Ortak Liste).
* **Product:** Sistemdeki tanımlı ürün kataloğu (Birim tipleri ile birlikte).

## 📄 Proje Raporu
Projenin analiz, tasarım ve geliştirme süreçlerine dair detaylar **Proje Raporu** dosya dizininde yer almaktadır.

*Bu proje üniversite bitirme/dönem ödevi kapsamında bir takım çalışması olarak geliştirilmiştir.*
