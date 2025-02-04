var app = angular.module("loginApp", []);

app.controller("LoginController", function ($scope, $http) {
  $scope.showLogin = false;
  $scope.showRegister = false;
  $scope.userType = "";

  //giriş popup açma
  $scope.openPopup = function (type) {
    $scope.userType = type === "student" ? "Student" : "Teacher";
    localStorage.setItem("userType", $scope.userType);
    $scope.showLogin = true;
    $scope.showRegister = false;
  };

  //kayıt popup açma
  $scope.openRegister = function () {
    $scope.showLogin = false;
    $scope.showRegister = true;
  };

  //giriş
  $scope.login = function () {
    console.log("Giriş fonksiyonu çalışıyor");

    var loginData = {
      email: $scope.email,
      password: $scope.password,
      expectedUserType: $scope.userType,
    };

    $http
      .post("/api/account/login", loginData)
      .then(function (response) {
        console.log("Giriş başarılı, yanıt:", response);

        if (response.status === 200) {
          var userType = response.data.userType;
          var userId = response.data.userId;

          if (!userId) {
            console.error("Kullanıcı ID'si bulunamadı!");
            alert("Giriş başarılı, ancak kullanıcı ID'si belirlenemedi.");
            return;
          }

          localStorage.setItem("userId", userId);
          localStorage.setItem("userType", userType);

          if (userType === "Teacher") {
            window.location.href = "/Home/Page";
          } else if (userType === "Student") {
            window.location.href = "/Home/Page";
          } else {
            console.error("Bilinmeyen kullanıcı tipi:", userType);
            alert("Giriş başarılı, ancak kullanıcı tipi belirlenemedi.");
          }
        }
      })
      .catch(function (error) {
        console.error("Giriş başarısız, hata:", error);
        alert(
          "Giriş başarısız: " +
            (error.data ? error.data.Message : "Bilinmeyen bir hata oluştu.")
        );
      });
  };

  //kayıt olma
  $scope.register = function () {
    var user = {
      firstName: $scope.firstName,
      lastName: $scope.lastName,
      email: $scope.email,
      password: $scope.password,
      userType: $scope.userType === "Öğrenci" ? "Student" : "Teacher",
    };

    //POST isteği ile verileri sunucuya gönderme
    $http.post("api/account/register", user).then(
      function (response) {
        alert("Kayıt başarılı!");
      },
      function (error) {
        alert("Kayıt sırasında bir hata oluştu: " + error.data);
      }
    );
  };
});
