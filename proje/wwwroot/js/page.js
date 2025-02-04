var app = angular.module("libraryApp", []);

app.controller("LibraryController", function ($scope, $http) {
  $scope.userType = localStorage.getItem("userType");
  $scope.userId = localStorage.getItem("userId");
  $scope.books = [];
  $scope.myLibrary = [];
  $scope.section = "books";
  $scope.newBook = {};
  $scope.selectedBookToDelete = null;
  $scope.selectedBook = null;
  $scope.newComment = "";
  $scope.comments = [];
  $scope.bookId = localStorage.getItem("bookId");
  $scope.genres = [];

  // Kullanıcı bilgilerini al ve `localStorage`'a kaydet
  $scope.loadUserInfo = function () {
    if ($scope.userId) {
      $http
        .get(`http://localhost:5107/api/users/${$scope.userId}`)
        .then(function (response) {
          const user = response.data;
          localStorage.setItem("userType", user.userType);
          $scope.userType = user.userType;
        })
        .catch(function (error) {
          console.log("Kullanıcı bilgilerini alırken hata:", error);
        });
    }
  };

  $scope.loadUserInfo();

  // Bölüm görüntüleme
  $scope.showSection = function (section) {
    $scope.section = section;
  };

  // Kitap detaylarını görüntüleme
  $scope.viewBook = function (book, section) {
    if (book._id || book.id) {
      localStorage.setItem("bookId", book._id || book.id);
      localStorage.setItem("previousSection", section); // Geçilen sayfayı sakla
      $scope.selectedBook = book;
      $scope.section = "bookDetails"; // Kitap detayları bölümüne geç
    } else {
      console.error("Kitap ID bulunamadı!");
    }
  };

  // Kitap detaylarından çıkış
  $scope.closeBookDetails = function () {
    var previousSection = localStorage.getItem("previousSection");
    if (previousSection === "myLibrary") {
      $scope.section = "myLibrary";
    } else if (previousSection === "books") {
      $scope.section = "books";
    }
  };

  $scope.loadGenres = function () {
    $http
      .get("http://localhost:5107/api/books/genres")
      .then(function (response) {
        $scope.genres = response.data; //veritabanından gelen türler
      })
      .catch(function (error) {
        console.error("Türler yüklenirken bir hata oluştu:", error);
      });
  };

  $scope.loadGenres(); //sayfa yüklendiğinde türleri yükle

  // Kitap ekleme
  $scope.addToLibrary = function (book) {
    if (!$scope.myLibrary.includes(book)) {
      $scope.myLibrary.push(book);
    }
    $scope.showSection("myLibrary");
  };

  // Yorum ekleme
  $scope.addComment = function () {
    const userIdString = $scope.userId ? $scope.userId.toString() : null;
    const bookIdlocal = $scope.selectedBook._id || $scope.selectedBook.id;
    console.log("Seçilen Kitap ID:", bookIdlocal);

    if (userIdString && bookIdlocal && $scope.newComment) {
      const commentData = {
        bookId: bookIdlocal, // selectedBook'dan alınan kitap ID'si
        userId: userIdString,
        comment: $scope.newComment,
        createdAt: new Date(),
      };

      $http
        .post("http://localhost:5107/api/comments", commentData)
        .then(function (response) {
          $scope.comments.push(response.data); // Yeni yorumu listeye ekle
          $scope.newComment = ""; // Yorum kutusunu temizle
        })
        .catch(function (error) {
          console.log("Yorum eklenirken hata:", error);
        });
    } else {
      alert("Kullanıcı ID, Kitap ID veya yorum içeriği eksik olamaz!");
    }
  };

  // Yorumları yükleme
  $scope.loadComment = function () {
    const bookId = localStorage.getItem("bookId");
    const userType = localStorage.getItem("userType"); //Kullanıcı türünü al

    if (bookId) {
      $http
        .get(`http://localhost:5107/api/comments?bookId=${bookId}`)
        .then(function (response) {
          if (response.data && Array.isArray(response.data)) {
            // Yalnızca kullanıcı tipi öğretmen olan yorumları yükle
            $scope.comments =
              userType === "Student"
                ? response.data.filter(
                    (comment) => comment.userId === $scope.userId
                  )
                : response.data; //Öğretmen ise tüm yorumları göster
          } else {
            console.error("Beklenmeyen yanıt verisi:", response.data);
          }
        })
        .catch(function (error) {
          console.error("Yorumları yüklerken hata oluştu:", error);
        });
    } else {
      console.error("Kitap ID eksik!");
    }
  };

  // Kitap ekleme
  $scope.addNewBook = function () {
    if (
      $scope.userType === "Teacher" &&
      $scope.newBook.title &&
      $scope.newBook.author
    ) {
      $http
        .post("http://localhost:5107/api/books", $scope.newBook)
        .then(function (response) {
          $scope.books.push(response.data);
          $scope.newBook = {};
          $scope.closeModal();
        })
        .catch(function (error) {
          console.log("Kitap eklenirken hata:", error);
        });
    } else {
      alert("Kitap ekleme yetkiniz yok veya kitap bilgileri eksik.");
    }
  };

  // Kitapları yükleme
  $scope.loadBooks = function () {
    $http
      .get("http://localhost:5107/api/books")
      .then(function (response) {
        $scope.books = response.data;
      })
      .catch(function (error) {
        console.log("Kitapları yüklerken hata oluştu:", error);
      });
  };

  //kitaplıkta yer alan kitapları kontrol etme fonksiyonu
  $scope.isInLibrary = function (bookId) {
    if (!$scope.myLibrary) {
      console.error("Kitaplık yüklenmedi veya boş!");
      return false;
    }

    //kitap kitaplıkta mı değil mi kontrolü
    const isInLibrary = $scope.myLibrary.some((book) => {
      return book._id === bookId || book.id === bookId;
    });
    return isInLibrary;
  };

  // Kitap ekleme fonksiyonu
  $scope.addBookToLibrary = function (bookId) {
    console.log("Fonksiyon çağrıldı. UserID:", $scope.userId);

    const userIdString = $scope.userId ? $scope.userId.toString() : null;
    const bookIdlocal = $scope.selectedBook._id || $scope.selectedBook.id;

    if (!userIdString || !bookIdlocal) {
      console.error("Kullanıcı ID veya Kitap ID eksik!");
      return;
    }

    console.log(
      `Kitap ekleme işlemi başlatılıyor, UserID: ${userIdString}, BookID: ${bookIdlocal}`
    );

    $http
      .post(
        `http://localhost:5107/api/userLibrary/${userIdString}/addBook`,
        JSON.stringify({ bookId: bookIdlocal })
      )
      .then(function (response) {
        console.log("Kitap başarıyla eklendi:", response.data);
        $scope.loadMyLibrary(); //kitap eklendikten sonra kitaplık yüklenir
      })
      .catch(function (error) {
        console.log("Kitap eklenirken hata oluştu:", error);
      });
  };

  //kitaplıktan çıkarma fonksiyonu
  $scope.removeBookFromLibrary = function (bookId) {
    const userIdString = $scope.userId.toString();
    const bookIdlocal = $scope.selectedBook._id || $scope.selectedBook.id;

    $http
      .delete(
        `http://localhost:5107/api/userLibrary/${userIdString}/removeBook/${bookIdlocal}`
      )
      .then(function (response) {
        console.log("Kitap başarıyla kitaplıktan çıkarıldı:", response.data);
        $scope.loadMyLibrary(); //kitaplıktan çıkarıldıktan sonra kitaplık güncelleniyor
      })
      .catch(function (error) {
        console.log("Kitaplıktan çıkarılırken hata oluştu:", error);
        if (error.data) {
          console.log("Sunucudan gelen hata:", error.data);
        }
      });
  };

  //kitaplığımı yükle
  $scope.loadMyLibrary = function () {
    if ($scope.userId) {
      const userIdString = $scope.userId.toString();
      console.log(`Kitaplık yükleniyor, UserID: ${userIdString}`);

      $http
        .get(`http://localhost:5107/api/userLibrary/${userIdString}`)
        .then(function (response) {
          console.log("Kitaplık başarıyla yüklendi:", response.data);
          $scope.myLibrary = response.data || []; // Kitaplık boşsa boş dizi atanıyor
          console.log("Güncel Kitaplık:", $scope.myLibrary);
        })
        .catch(function (error) {
          console.log("Kullanıcı kitaplığı yüklenirken hata:", error);
        });
    } else {
      console.error("Kullanıcı ID eksik!");
    }
  };

  // selectedBook değiştiğinde butonların durumunu güncelle
  $scope.$watch("selectedBook", function (newVal, oldVal) {
    if (newVal && newVal._id) {
      // Kitap seçildiğinde ilgili butonların güncellenmesini sağla
      $scope.isInLibrary(newVal._id);
    }
  });

  // Sayfa yüklendiğinde kitaplık yükleniyor
  $scope.loadMyLibrary();

  // Kitap silme
  $scope.deleteBook = function () {
    if ($scope.selectedBookToDelete) {
      const bookId = $scope.selectedBookToDelete.id;
      if (bookId) {
        $http
          .delete(`/api/books/${bookId}`)
          .then(function () {
            $scope.loadBooks();
          })
          .catch(function (error) {
            console.error("Kitap silinirken bir hata oluştu", error);
          });
      } else {
        alert("Kitap ID bulunamadı");
      }
    } else {
      alert("Lütfen bir kitap seçin");
    }
  };

  // Seçilen kitabı doldur
  $scope.populateBookDetails = function (book) {
    $scope.selectedBook = angular.copy(book);
  };

  // Kitap güncelleme fonksiyonu
  $scope.updateBook = function () {
    if ($scope.userType === "Teacher" && $scope.selectedBook) {
      $http
        .put(
          "http://localhost:5107/api/books/" + $scope.selectedBook.id,
          $scope.selectedBook
        )
        .then(function (response) {
          //güncellenen kitabı listeye yansıtmak için
          const index = $scope.books.findIndex(
            (b) => b.id === $scope.selectedBook.id
          );
          if (index !== -1) {
            $scope.books[index] = response.data; //listeyi güncellemek için
          }
          $scope.closeModal();
        })
        .catch(function (error) {
          console.log("Kitap güncellenirken hata:", error);
        });
    } else {
      alert("Güncelleme yetkiniz yok veya kitap seçilmedi.");
    }
  };

  // Modal kapatma fonksiyonu
  $scope.closeModal = function () {
    $scope.section = null; // Sekmeyi sıfırla
    $scope.selectedBook = null; // Seçimi sıfırla
  };

  // Modal kapatma
  $scope.closeModal = function () {
    $scope.section = "bookOperations";
  };

  //çıkış
  document
    .getElementById("logoutButton")
    .addEventListener("click", function (event) {
      event.preventDefault();
      fetch("/api/account/logout", {
        method: "POST",
      })
        .then((response) => {
          if (response.ok) {
            localStorage.removeItem("userId");
            localStorage.removeItem("userType");
            localStorage.removeItem("bookId");

            window.location.href = "/"; // İlk sayfaya yönlendir
          }
        })
        .catch((error) => console.log(error));
    });

  // Başlangıçta kitapları ve kitaplığı yükle
  $scope.loadBooks();
  $scope.loadMyLibrary();
});
