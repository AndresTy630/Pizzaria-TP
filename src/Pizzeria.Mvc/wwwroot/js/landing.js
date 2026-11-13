document.addEventListener("DOMContentLoaded", function () {
  // MENÚ RESPONSIVE

  const boton = document.getElementById("menuToggle");
  const enlaces = document.getElementById("navLinks");

  boton.addEventListener("click", function () {
    enlaces.classList.toggle("show");
  });

  // CARRUSEL

  const slides = document.querySelectorAll(".hero-slide");

  let actual = 0;

  setInterval(function () {
    slides[actual].classList.remove("active");

    actual++;

    if (actual >= slides.length) {
      actual = 0;
    }

    slides[actual].classList.add("active");
  }, 5000);
});
