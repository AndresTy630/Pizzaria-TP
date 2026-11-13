document.addEventListener("DOMContentLoaded", function () {
  // MENÚ RESPONSIVE
  const navToggle = document.getElementById("navToggle");
  const navLinks = document.getElementById("navLinks");

  if (navToggle && navLinks) {
    navToggle.addEventListener("click", function () {
      navLinks.classList.toggle("show");
    });

    const enlaces = navLinks.querySelectorAll("a");

    enlaces.forEach(function (enlace) {
      enlace.addEventListener("click", function () {
        navLinks.classList.remove("show");
      });
    });
  }

  // CARRUSEL AUTOMÁTICO
  const slides = document.querySelectorAll(".hero-slide");
  const slideNumber = document.getElementById("slideNumber");

  if (slides.length > 0) {
    let actual = 0;

    function cambiarSlide() {
      slides[actual].classList.remove("active");

      actual++;

      if (actual >= slides.length) {
        actual = 0;
      }

      slides[actual].classList.add("active");

      if (slideNumber) {
        slideNumber.textContent = String(actual + 1).padStart(2, "0");
      }
    }

    setInterval(cambiarSlide, 5500);
  }
});
