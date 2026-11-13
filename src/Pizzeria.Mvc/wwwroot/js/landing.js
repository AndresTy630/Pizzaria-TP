document.addEventListener("DOMContentLoaded", function () {
  // MENÚ RESPONSIVE
  const boton = document.getElementById("menuToggle");
  const enlaces = document.getElementById("navLinks");

  if (boton && enlaces) {
    boton.addEventListener("click", () => enlaces.classList.toggle("show"));
  }

  // CARRUSEL (desliza)
  const track = document.getElementById("heroTrack");

  if (track) {
    const total = track.children.length;
    let actual = 0;

    setInterval(function () {
      actual = (actual + 1) % total;
      track.style.transform = `translateX(-${actual * 100}%)`;
    }, 5000);
  }
});
