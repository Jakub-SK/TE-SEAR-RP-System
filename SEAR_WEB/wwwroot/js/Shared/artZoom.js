const images = [
    "/images/Shared/RayaArt/raya1.jpg",
    "/images/Shared/RayaArt/raya2.jpg",
    "/images/Shared/RayaArt/raya3.jpg",
    "/images/Shared/RayaArt/raya4.jpg",
    "/images/Shared/RayaArt/raya5.jpg",
    "/images/Shared/RayaArt/raya6.jpg",
    "/images/Shared/RayaArt/raya7.jpg",
    "/images/Shared/RayaArt/raya8.jpg"
];

// ^ up above is ugly and will need to be changed for the formatted link we use instead

// preload images to hopefully make zoom less choppy
images.forEach(src => {
    const img = new Image();
    img.src = src;
});

let current = document.querySelector(".current");
let next = document.querySelector(".next");

let index = Math.floor(Math.random() * images.length);
let startTime = null;
let isFading = false;

const duration = 20000;      // 20 seconds per image
const fadeDuration = 1500;   // 1.5 seconds before zoom starts to give browser time to breathe
const maxZoom = 1.15; // desired zoom, lower value like this looks smoother

current.style.backgroundImage = `url(${images[index]})`;
current.classList.add("active");

function animate(timestamp) {
    if (!startTime) startTime = timestamp;

    const progress = timestamp - startTime;
    const raw = Math.min(progress / duration, 1);

    // stolen code to make zoom smoother
    const eased = raw < 0.5
        ? 4 * raw * raw * raw
        : 1 - Math.pow(-2 * raw + 2, 3) / 2;

    const zoom = 1 + (maxZoom - 1) * eased;
    current.style.transform = `scale(${zoom})`;
    
    // fading
    if (!isFading && progress >= duration - fadeDuration) {
        isFading = true;

        const nextIndex = (index + 1) % images.length;
        next.style.backgroundImage = `url(${images[nextIndex]})`;

        next.classList.add("active");
        current.classList.remove("active");
    }

    // swapping
    if (progress < duration) {
        requestAnimationFrame(animate);
    } 
    else {
       
        index = (index + 1) % images.length;

        current.style.transform = "scale(1)";
        [current, next] = [next, current];

        startTime = null;
        isFading = false;

        requestAnimationFrame(animate);
    }
}

requestAnimationFrame(animate);