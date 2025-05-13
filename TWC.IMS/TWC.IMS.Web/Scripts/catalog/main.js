// ✨ Load catalog data dynamically instead of using data.js
// ✅ Initialize empty data to avoid ReferenceErrors if accessed early
window.data = {
	videos: {},
	carousel: [],
	products: {},
	other: {}
};

// ✨ Load catalog data dynamically instead of using data.js
async function loadCatalogData() {
	try {
		const [videoRes, carouselRes, productRes] = await Promise.all([
			fetch('/Catalog/GetVideos'),
			fetch('/Catalog/GetCarousels'),
			fetch('/Catalog/GetProducts')
		]);

		const videos = await videoRes.json();
		const carousel = await carouselRes.json();
		const products = await productRes.json();

		window.data = {
			videos,
			carousel,
			products: groupProductsByCategory(products), // Group before use
			other: {}
		};

		// Re-render all dynamic content
		renderVideoCategories();
		renderProductCategories();
		renderOtherCategories(); // Optional
	} catch (error) {
		console.error('❌ Failed to fetch catalog data:', error);
		const fallback = document.getElementById("product-sections");
		if (fallback) {
			fallback.innerHTML = `<p class="text-danger text-center">Unable to load catalog. Please try again later.</p>`;
		}
	}
}

document.addEventListener('DOMContentLoaded', () => {
	loadCatalogData();
});

// ✅ Helper to group products by category
function groupProductsByCategory(products) {
	const grouped = {};
	products.forEach(p => {
		const category = (p.category || "Uncategorized").toLowerCase();
		if (!grouped[category]) grouped[category] = [];
		grouped[category].push(p);
	});
	return grouped;
}


document.addEventListener('DOMContentLoaded', () => {
	loadCatalogData();
});



function mobileCheck() {
	var winWidth=$(window).width();
	if (winWidth<=768) {
		$("#sidebar").after($("#body .pagination:first"))
	} else {
		$(".products-wrap").before($("#body .pagination:first"))
	}
}

$(document).ready(function() {
	$("input[type=checkbox]").crfi();
	$("select").crfs();
	$("#slider ul").bxSlider({
		controls: false,
		auto: true,
		mode: 'fade',
		preventDefaultSwipeX: false
	});
	$(".last-products .products").bxSlider({
		pager: false,
		minSlides: 1,
		maxSlides: 5,
		slideWidth: 235,
		slideMargin: 0
	});


	$(".tabs .nav a").click(function() {
		var container = $(this).parentsUntil(".tabs").parent();
		if (!$(this).parent().hasClass("active")) {
			container.find(".nav .active").removeClass("active")
			$(this).parent().addClass("active")
			container.find(".tab-content").hide()
			$($(this).attr("href")).show();
		};
		return false;
	});
	$("#price-range").slider({
		range: true,
		min: 0,
		max: 5000,
		values: [ 500, 3500 ],
		slide: function( event, ui ) {
			$(".ui-slider-handle:first").html("<span>$" + ui.values[ 0 ] + "</span>");
			$(".ui-slider-handle:last").html("<span>$" + ui.values[ 1 ] + "</span>");
		}
	});
	$(".ui-slider-handle:first").html("<span>$" + $( "#price-range" ).slider( "values", 0 ) + "</span>");
	$(".ui-slider-handle:last").html("<span>$" + $( "#price-range" ).slider( "values", 1 ) + "</span>");
	$("#menu .trigger").click(function() {
		$(this).toggleClass("active").next().toggleClass("active")
	});

	mobileCheck();
	$(window).resize(function() {
		mobileCheck();
	});
});


function navigate(sectionId) {
	document.querySelectorAll('.section').forEach(section => {
		section.classList.remove('active');
	});
	
	document.getElementById('loader').style.display = 'block';
	
	setTimeout(() => {
		document.getElementById('loader').style.display = 'none';
		document.getElementById(sectionId).classList.add('active');
		triggerFunctions(sectionId);
	}, 500);
}

function triggerFunctions(sectionId) {
	if (sectionId === 'home') {
		homeFunction();
	} else if (sectionId === 'about') {
		aboutFunction();
	} else if (sectionId === 'contact') {
		contactFunction();
	} else if (sectionId === 'sets') {
		productFunction('sets');
	} else if (sectionId === 'earrings') {
		productFunction('earrings');
	} else if (sectionId === 'pendants') {
		productFunction('pendants');
	} else if (sectionId === 'rings') {
		productFunction('rings');
	} else if (sectionId === 'new-arrival') {
		productFunction('new-arrival');
	}


}

function homeFunction() {
	
	console.log('Home section loaded');
}

function aboutFunction() {
	console.log('About section loaded');
}

function contactFunction() {
	console.log('Contact section loaded');
}


const infoSections = [
	{
		title: "Order & Purchase Process",
		content: `Thank you for showing interest in our product. To properly serve you better, you may visit us at our Boutique located at:<br><br>
📍 1466 San Miguel Building, CM Recto Ave, Sta Cruz, Manila, Philippines or contact us directly on our main channels:<br><br>
Messenger<br>
Viber<br>
Sales Representative you might already be in contact with.<br>
Join our growing VIBER Community <a href="#">CLICK HERE</a>`
	},
	{
		title: "Service Warranty",
		content: `All our watches come with a limited 1 Year Service Warranty. Terms and Conditions Applied. Please contact our Customer Service Representative or Sales Representative for more information.<br><br>
Join our growing VIBER Community <a href="#">CLICK HERE</a>`
	},
	{
		title: "Payment Methods",
		content: `• Trades Accepted (Trade Value Higher than Cash Price)<br>
• Cash, Gcash, Bank Transfer, QR Code payments all accepted.<br>
• Credit Card payments are now accepted.<br>
• All Photos are actual photos shot and owned exclusively by Darlings FineJewels By Candys.<br><br>
• Unit reserved with down payments (reservation fee) are considered sold. Down payments are non-refundable<br><br>
Join our growing VIBER Community <a href="#">CLICK HERE</a>`
	}
];


function productFunction(category) {
	if (typeof data === 'undefined') {
		console.error("Error: 'data' is not defined. Make sure data.js is loaded.");
		return;
	}

	if (!data.products[category]) {
		console.error(`Error: Category '${category}' not found in data.js`);
		return;
	}

	const productGrid = document.getElementById(category + '-product-grid');
	if (!productGrid) {
		console.error("Error: Element with ID 'product-grid' not found in the DOM.");
		return;
	}

	productGrid.innerHTML = "";
	const products = data.products[category];

	if (!Array.isArray(products) || products.length === 0) {
		productGrid.innerHTML = `<p class="text-center">No products available in this category.</p>`;
		return;
	}

	let productCards = "";
	products.forEach(product => {
		const col = document.createElement("div");
		col.classList.add("col");

		const card = document.createElement("div");
		card.classList.add("card", "product-card", "zoom-card");

		const img = document.createElement("img");
		img.src = product.file_location;
		img.classList.add("card-img-top");
		img.alt = product.name;

		const cardBody = document.createElement("div");
		cardBody.classList.add("card-body");

		const title = document.createElement("h5");
		title.classList.add("card-title");
		title.textContent = product.name;

		const price = document.createElement("h5");
		price.classList.add("card-title");
		price.textContent = `₱${product.price}`;

		cardBody.appendChild(title);
		cardBody.appendChild(price);
		card.appendChild(img);
		card.appendChild(cardBody);
		col.appendChild(card);
		productGrid.appendChild(col);

		card.addEventListener("click", () => {
			const zoomOverlay = document.getElementById('zoom-overlay');
			document.getElementById('zoomed-image').src = product.file_location;
			document.getElementById('zoomed-name').textContent = product.name;
			document.getElementById('zoomed-price').textContent = `₱${product.price}`;

			const availabilityInfo = document.getElementById('zoomed-availability');
			availabilityInfo.classList.add('availability-badge');
			availabilityInfo.textContent = 'Available';
			availabilityInfo.style.color = '#155724';

			const infoContainer = document.getElementById('zoomed-extra-info');
			infoContainer.innerHTML = '';
			infoSections.forEach(section => {
				infoContainer.appendChild(createExpandableSection(section.title, section.content));
			});

			// Populate thumbnails
			const thumbnailContainer = document.getElementById('zoom-thumbnails');
			thumbnailContainer.innerHTML = '';
			const recentThumbs = products;
			recentThumbs.forEach(thumbProduct => {
				const thumbImg = document.createElement('img');
				thumbImg.src = thumbProduct.file_location;
				thumbImg.alt = thumbProduct.name;

				thumbImg.addEventListener('click', () => {
					document.getElementById('zoomed-image').src = thumbProduct.file_location;
					document.getElementById('zoomed-name').textContent = thumbProduct.name;
					document.getElementById('zoomed-price').textContent = `₱${thumbProduct.price}`;

					const infoContainer = document.getElementById('zoomed-extra-info');
					infoContainer.innerHTML = '';
					infoSections.forEach(section => {
						infoContainer.appendChild(createExpandableSection(section.title, section.content));
					});
				});

				thumbnailContainer.appendChild(thumbImg);
			});

			
			zoomOverlay.style.display = 'flex';
		});
	});
}



function videoFunction(category) {
    if (typeof data === 'undefined') {
        console.error("Error: 'data' is not defined. Make sure data.js is loaded.");
        return;
    }

    if (!data.videos[category]) {
        console.error(`Error: Category '${category}' not found in data.js`);
        return;
    }

    const videoGrid = document.getElementById(category + '-video-grid');
    if (!videoGrid) {
        console.error("Error: Element with ID '" + category + "-video-grid' not found in the DOM.");
        return;
    }

    videoGrid.innerHTML = ""; // Clear the previous videos

    const videos = data.videos[category];
    console.log(videos);

    if (!Array.isArray(videos) || videos.length === 0) {
        videoGrid.innerHTML = `<p class="text-center">No videos available in this category.</p>`;
        return;
    }

    let videoCards = "";
    videos.forEach(video => {
        videoCards += `
            <div class="col">
                <div class="card video-card">
                    <div class="video-wrapper">
                        <video class="modern-video" controls>
                            <source src="${video.file_location}" type="video/mp4">
                            Your browser does not support the video tag.
                        </video>
                    </div>
                    <div class="card-body">
                        <h5 class="card-title">${video.name}</h5>
                    </div>
                </div>
            </div>
        `;
    });

    videoGrid.innerHTML = videoCards;
}


function otherFunction(category) {
	if (typeof data === 'undefined') {
		console.error("Error: 'data' is not defined. Make sure data.js is loaded.");
		return;
	}

	if (!data.products[category]) {
		console.error(`Error: Category '${category}' not found in data.js`);
		return;
	}


	const productGrid = document.getElementById(category+'-other-grid');
	if (!productGrid) {
		console.error("Error: Element with ID 'product-grid' not found in the DOM.");
		return;
	}

	productGrid.innerHTML = ""; 

	const products = data.other[category]; 
	console.log(products);

	if (!Array.isArray(products) || products.length === 0) {
		productGrid.innerHTML = `<p class="text-center">No products available in this category.</p>`;
		return;
	}

	

	let productCards = "";
	products.forEach(product => {
		productCards += `
			<div class="col">
				<div class="card product-card">
					<img src="${product.file_location}" class="card-img-top" alt="${product.name}">
					<div class="card-body">
						<h5 class="card-title">${product.name}</h5>
						<h5 class="card-title">${product.price}</h5>
					</div>
				</div>
			</div>`;
	});

	productGrid.innerHTML = productCards;
}



const createExpandableSection = (title, content) => {
	const section = document.createElement("div");
	section.classList.add("expand-section");

	const header = document.createElement("div");
	header.classList.add("expand-header");
	header.innerHTML = `<span class="toggle-symbol">+</span> ${title}`;

	const contentDiv = document.createElement("div");
	contentDiv.classList.add("expand-content");
	contentDiv.innerHTML = content;

	header.onclick = () => {
		contentDiv.classList.toggle("active");
		header.querySelector('.toggle-symbol').textContent = contentDiv.classList.contains("active") ? "−" : "+";
	};

	section.appendChild(header);
	section.appendChild(contentDiv);
	return section;
};
const renderProductCategories = () => {
	const productSectionsContainer = document.getElementById("product-sections");

	for (const category in data.products) {
		const categoryData = data.products[category];
		const lastFiveProducts = categoryData.slice(-5);

		const categoryTitle = document.createElement('div');
		categoryTitle.classList.add('category-title');

		const categoryLink = document.createElement('a');
		categoryLink.href = "#";
		categoryLink.textContent = category.charAt(0).toUpperCase() + category.slice(1);
		categoryLink.onclick = () => navigate(category.toLowerCase());

		categoryTitle.appendChild(categoryLink);

		const productContainer = document.createElement('div');
		productContainer.classList.add('product-page-products');

		lastFiveProducts.forEach(product => {
			const productCard = document.createElement('div');
			productCard.classList.add('product-page-product-card', 'zoom-card');

			productCard.addEventListener('click', () => {
				const zoomOverlay = document.getElementById('zoom-overlay');
				document.getElementById('zoomed-image').src = product.file_location;
				document.getElementById('zoomed-name').textContent = product.name;
				document.getElementById('zoomed-price').textContent = `₱${product.price}`;

				const availabilityInfo = document.getElementById('zoomed-availability');
				availabilityInfo.classList.add('availability-badge');
				availabilityInfo.textContent = 'Available';
				availabilityInfo.style.color = '#155724';
				availabilityInfo.style.fontSize = '1.2rem';

				// Extra info
				const infoContainer = document.getElementById('zoomed-extra-info');
				infoContainer.innerHTML = '';
				infoSections.forEach(section => {
					infoContainer.appendChild(createExpandableSection(section.title, section.content));
				});

				// Populate thumbnails
				const thumbnailContainer = document.getElementById('zoom-thumbnails');
				thumbnailContainer.innerHTML = '';

				lastFiveProducts.forEach(thumbProduct => {
					const thumbImg = document.createElement('img');
					thumbImg.src = thumbProduct.file_location;
					thumbImg.alt = thumbProduct.name;

					thumbImg.addEventListener('click', () => {
						document.getElementById('zoomed-image').src = thumbProduct.file_location;
						document.getElementById('zoomed-name').textContent = thumbProduct.name;
						document.getElementById('zoomed-price').textContent = `₱${thumbProduct.price}`;

						const availabilityInfo = document.getElementById('zoomed-availability');
						availabilityInfo.textContent = 'Available';
						availabilityInfo.style.color = '#155724';

						const infoContainer = document.getElementById('zoomed-extra-info');
						infoContainer.innerHTML = '';
						infoSections.forEach(section => {
							infoContainer.appendChild(createExpandableSection(section.title, section.content));
						});
					});

					thumbnailContainer.appendChild(thumbImg);
				});

				// Setup See More link
const seeMoreLink = document.getElementById('zoom-see-more');
if (seeMoreLink) {
	seeMoreLink.href = "#";
	seeMoreLink.textContent = `See more ${category}`;
	seeMoreLink.onclick = () => {
		navigate(category.toLowerCase());
		zoomOverlay.style.display = 'none';
	};
}

				zoomOverlay.style.display = 'flex';
			});

			const productImage = document.createElement('img');
			productImage.src = product.file_location;
			productImage.alt = product.name;

			const productName = document.createElement('h3');
			productName.textContent = product.name;

			const productPrice = document.createElement('div');
			productPrice.classList.add('price');
			productPrice.textContent = `₱${product.price}`;

			productCard.appendChild(productImage);
			productCard.appendChild(productPrice);
			productCard.appendChild(productName);
			productContainer.appendChild(productCard);
		});

		productSectionsContainer.appendChild(categoryTitle);
		productSectionsContainer.appendChild(productContainer);
	}
};

/*renderProductCategories();*/

document.getElementById('close-zoom').addEventListener('click', () => {
    document.getElementById('zoom-overlay').classList.remove('active');
});
const renderVideoCategories = () => {
    const videoSectionsContainer = document.getElementById("video-sections");

    for (const category in data.videos) {
        const categoryData = data.videos[category];

        // --- Category title (above the section content) ---
        const categoryTitle = document.createElement('div');
        categoryTitle.classList.add('category-title');
        categoryTitle.textContent = category.charAt(0).toUpperCase() + category.slice(1);
        videoSectionsContainer.appendChild(categoryTitle);

        // --- Main Section Card Wrapper ---
        const sectionCard = document.createElement('div');
        sectionCard.classList.add('video-section-card');

        // --- Narration block ---
        const narrationWrapper = document.createElement('div');
        narrationWrapper.classList.add('video-narration');

        const narrationTitle = document.createElement('h3');
        narrationTitle.classList.add('narration-title');

        const narrationDescription = document.createElement('p');
        narrationDescription.classList.add('narration-description');

        if (category.toLowerCase() === 'colaboration') {
            narrationTitle.textContent = 'Discover with Mary Carino & Pareng Hayb';
            narrationDescription.textContent = 'Join us on an exciting journey as we team up with popular vloggers Mary Carino and Pareng Hayb! From behind-the-scenes moments to exclusive product features, discover the stories, elegance, and fun that make our shop a must-visit. Dive into their vlogs and see what makes this collab extra special!';
        } else if (category.toLowerCase() === 'store') {
            narrationTitle.textContent = 'Sparkle & Shine with Candy’s Jewelry';
            narrationDescription.textContent = 'Step into the world of elegance at Candy’s Jewelry Store! From dainty everyday pieces to dazzling statement jewelry, we’ve got the perfect sparkle for every style and occasion. Browse our curated collection, shop with ease, and let your beauty shine brighter with every piece.';
        }

        narrationWrapper.appendChild(narrationTitle);
        narrationWrapper.appendChild(narrationDescription);
        sectionCard.appendChild(narrationWrapper);

        // --- Main Video ---
        const mainVideoWrapper = document.createElement('div');
        mainVideoWrapper.classList.add('main-video-wrapper');

        const mainVideoTitle = document.createElement('div');
        mainVideoTitle.classList.add('main-video-title');
        mainVideoTitle.textContent = categoryData[0].name;

        const mainVideoCard = document.createElement('div');
        mainVideoCard.classList.add('main-video-card');

        const mainVideo = document.createElement('video');
        mainVideo.classList.add('modern-video');
        mainVideo.setAttribute('controls', 'true');
        mainVideo.setAttribute('muted', 'true');
        mainVideo.setAttribute('playsinline', 'true');

        const mainVideoSource = document.createElement('source');
        mainVideoSource.setAttribute('src', categoryData[0].file_location);
        mainVideoSource.setAttribute('type', 'video/mp4');
        mainVideo.appendChild(mainVideoSource);

        mainVideoCard.appendChild(mainVideo);
        
        // --- Thumbnails inside the main video card ---
        const thumbnailsContainer = document.createElement('div');
        thumbnailsContainer.classList.add('thumbnails-container');

        const allVideos = [];

        categoryData.forEach((video, index) => {
            const thumbnailCard = document.createElement('div');
            thumbnailCard.classList.add('thumbnail-card');
            if (index === 0) thumbnailCard.classList.add('selected');

            const miniVideo = document.createElement('video');
            miniVideo.classList.add('video-thumbnail');
            miniVideo.setAttribute('src', video.file_location);
            miniVideo.setAttribute('muted', 'true');
            miniVideo.setAttribute('loop', 'true');
            miniVideo.setAttribute('playsinline', 'true');
            miniVideo.setAttribute('title', video.name);

            // Generate and set poster from the first frame of the video
            const canvas = document.createElement('canvas');
            const ctx = canvas.getContext('2d');

            const videoElement = document.createElement('video');
            videoElement.src = video.file_location;
            videoElement.addEventListener('loadeddata', () => {
                canvas.width = videoElement.videoWidth;
                canvas.height = videoElement.videoHeight;
                videoElement.currentTime = 0; // Get the first frame

                videoElement.addEventListener('seeked', () => {
                    ctx.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
                    const dataUrl = canvas.toDataURL();
                    miniVideo.setAttribute('poster', dataUrl); // Set the first frame as poster
                });
            });

            allVideos.push(miniVideo);

            thumbnailCard.addEventListener('click', () => {
                allVideos.forEach(v => v.pause());
                mainVideo.pause();
                mainVideo.innerHTML = '';

                const newSource = document.createElement('source');
                newSource.setAttribute('src', video.file_location);
                newSource.setAttribute('type', 'video/mp4');
                mainVideo.appendChild(newSource);
                mainVideo.load();
                mainVideo.play();

                mainVideoTitle.textContent = video.name;
                updateThumbnailSelection(thumbnailCard);
            });

            thumbnailCard.appendChild(miniVideo);
            thumbnailsContainer.appendChild(thumbnailCard);
        });

        // Add thumbnails to the main video card
        mainVideoCard.appendChild(thumbnailsContainer);
        mainVideoWrapper.appendChild(mainVideoTitle);
        mainVideoWrapper.appendChild(mainVideoCard);

        sectionCard.appendChild(mainVideoWrapper);
        videoSectionsContainer.appendChild(sectionCard);
    }
};

const updateThumbnailSelection = (selectedThumbnailCard) => {
    document.querySelectorAll('.thumbnail-card').forEach(card => card.classList.remove('selected'));
    selectedThumbnailCard.classList.add('selected');
};

document.addEventListener('DOMContentLoaded', () => {
    renderVideoCategories();
});



const renderOtherCategories = () => {
	const productSectionsContainer = document.getElementById("other-sections");

	for (const category in data.other) {
		const categoryData = data.other[category];
		const lastFiveProducts = categoryData.slice(-5);

		const categoryTitle = document.createElement('div');
		categoryTitle.classList.add('category-title');

		const categoryLink = document.createElement('a');
		categoryLink.href = "#";
		categoryLink.textContent = category.charAt(0).toUpperCase() + category.slice(1);
		categoryLink.onclick = () => navigate(category.toLowerCase());

		categoryTitle.appendChild(categoryLink);

		const productContainer = document.createElement('div');
		productContainer.classList.add('product-page-products');

		lastFiveProducts.forEach(product => {
			const productCard = document.createElement('div');
			productCard.classList.add('product-page-product-card', 'zoom-card');

			productCard.addEventListener('click', () => {
				const zoomOverlay = document.getElementById('zoom-overlay');
				document.getElementById('zoomed-image').src = product.file_location;
				document.getElementById('zoomed-name').textContent = product.name;
				document.getElementById('zoomed-price').textContent = `₱${product.price}`;

				const availabilityInfo = document.getElementById('zoomed-availability');
				availabilityInfo.classList.add('availability-badge');
				availabilityInfo.textContent = 'Available';
				availabilityInfo.style.color = '#155724';
				availabilityInfo.style.fontSize = '1.2rem';

				// Extra info
				const infoContainer = document.getElementById('zoomed-extra-info');
				infoContainer.innerHTML = '';
				infoSections.forEach(section => {
					infoContainer.appendChild(createExpandableSection(section.title, section.content));
				});

				// Populate thumbnails
				const thumbnailContainer = document.getElementById('zoom-thumbnails');
				thumbnailContainer.innerHTML = '';

				lastFiveProducts.forEach(thumbProduct => {
					const thumbImg = document.createElement('img');
					thumbImg.src = thumbProduct.file_location;
					thumbImg.alt = thumbProduct.name;

					thumbImg.addEventListener('click', () => {
						document.getElementById('zoomed-image').src = thumbProduct.file_location;
						document.getElementById('zoomed-name').textContent = thumbProduct.name;
						document.getElementById('zoomed-price').textContent = `₱${thumbProduct.price}`;

						const availabilityInfo = document.getElementById('zoomed-availability');
						availabilityInfo.textContent = 'Available';
						availabilityInfo.style.color = '#155724';

						const infoContainer = document.getElementById('zoomed-extra-info');
						infoContainer.innerHTML = '';
						infoSections.forEach(section => {
							infoContainer.appendChild(createExpandableSection(section.title, section.content));
						});
					});

					thumbnailContainer.appendChild(thumbImg);
				});

				// Setup See More link
const seeMoreLink = document.getElementById('zoom-see-more');
if (seeMoreLink) {
	seeMoreLink.href = "#";
	seeMoreLink.textContent = `See more ${category}`;
	seeMoreLink.onclick = () => {
		navigate(category.toLowerCase());
		zoomOverlay.style.display = 'none';
	};
}

				zoomOverlay.style.display = 'flex';
			});

			const productImage = document.createElement('img');
			productImage.src = product.file_location;
			productImage.alt = product.name;

			const productName = document.createElement('h3');
			productName.textContent = product.name;

			const productPrice = document.createElement('div');
			productPrice.classList.add('price');
			productPrice.textContent = `₱${product.price}`;

			productCard.appendChild(productImage);
			productCard.appendChild(productPrice);
			productCard.appendChild(productName);
			productContainer.appendChild(productCard);
		});

		productSectionsContainer.appendChild(categoryTitle);
		productSectionsContainer.appendChild(productContainer);
	}
};

renderOtherCategories();


document.querySelector('.zoom-close').addEventListener('click', function () {
	document.getElementById('zoom-overlay').style.display = 'none';
});

document.getElementById('zoom-overlay').addEventListener('click', function (e) {
	if (e.target === this) {
		this.style.display = 'none';
	}
});

	
function toggleVideo(videoId) {
  const video = document.getElementById(videoId);

  if (video.paused) {
    video.play();
    video.muted = false; // unmute if desired
  } else {
    video.pause();
  }
}


const slides = document.querySelectorAll(".carousel-slide");
const prevBtn = document.querySelector(".carousel-btn.prev");
const nextBtn = document.querySelector(".carousel-btn.next");
const dotsContainer = document.querySelector(".carousel-dots");

let currentIndex = 0;
let autoSlideInterval;

// Create dots dynamically
slides.forEach((_, i) => {
  const dot = document.createElement("button");
  dot.addEventListener("click", () => goToSlide(i));
  dotsContainer.appendChild(dot);
});
const dots = dotsContainer.querySelectorAll("button");

function showSlide(index) {
  slides.forEach((slide, i) => {
    slide.classList.toggle("active", i === index);
    dots[i].classList.toggle("active", i === index);
  });
}

function nextSlide() {
  currentIndex = (currentIndex + 1) % slides.length;
  showSlide(currentIndex);
}

function prevSlide() {
  currentIndex = (currentIndex - 1 + slides.length) % slides.length;
  showSlide(currentIndex);
}

function goToSlide(index) {
  currentIndex = index;
  showSlide(currentIndex);
}

function startAutoSlide() {
  autoSlideInterval = setInterval(nextSlide, 3000);
}

function stopAutoSlide() {
  clearInterval(autoSlideInterval);
}

// Init
showSlide(currentIndex);
startAutoSlide();

prevBtn.addEventListener("click", () => {
  prevSlide();
  stopAutoSlide();
  startAutoSlide();
});
nextBtn.addEventListener("click", () => {
  nextSlide();
  stopAutoSlide();
  startAutoSlide();
});

document.querySelector(".custom-carousel").addEventListener("mouseenter", stopAutoSlide);
document.querySelector(".custom-carousel").addEventListener("mouseleave", startAutoSlide);

// Get the checkbox and the body element
  const themeToggle = document.getElementById('theme-toggle');
  const body = document.body;

  // Check if a theme is already stored in localStorage
  const savedTheme = localStorage.getItem('theme');

  // Apply the saved theme if present
  if (savedTheme) {
    body.classList.add(savedTheme);
    if (savedTheme === 'grey-theme') {
      themeToggle.checked = true; // If it's grey theme, check the toggle
    }
  } else {
    // Apply default theme
    body.classList.add('rustic-theme');
  }

  // Add event listener to the toggle checkbox
  themeToggle.addEventListener('change', () => {
    if (themeToggle.checked) {
      // Switch to grey theme
      body.classList.remove('rustic-theme');
      body.classList.add('grey-theme');
      localStorage.setItem('theme', 'grey-theme');
    } else {
      // Switch to rustic theme
      body.classList.remove('grey-theme');
      body.classList.add('rustic-theme');
      localStorage.setItem('theme', 'rustic-theme');
    }
  });

  window.addEventListener('scroll', function () {
    const parallax = document.querySelector('.jumbotron-parallax3');
    if (parallax) {
      const offset = window.pageYOffset;
      parallax.style.backgroundPositionY = offset * 0.5 + "px";
    }
  });

document.addEventListener("DOMContentLoaded", function () {
  const video = document.getElementById("portraitVideo");

  if ("IntersectionObserver" in window && video) {
    const observer = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          video.play();
        } else {
          video.pause();
        }
      });
    }, { threshold: 0.5 });

    observer.observe(video);
  }
});


document.addEventListener("DOMContentLoaded", function () {
  const video = document.getElementById("hero-video");

  if ("IntersectionObserver" in window && video) {
    const observer = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          video.play();
        } else {
          video.pause();
        }
      });
    }, { threshold: 0.5 });

    observer.observe(video);
  }
});

function openZoom(element) {
  const name = element.getAttribute('data-name');
  const price = element.getAttribute('data-price');
  const img = element.getAttribute('data-img');
  const availability = element.getAttribute('data-availability');

  // Set values in zoom overlay
  document.getElementById('zoomed-name').textContent = name;
  document.getElementById('zoomed-price').textContent = price;
  document.getElementById('zoomed-image').src = img;
  document.getElementById('zoomed-availability').textContent = availability;

  // Show zoom overlay
  document.getElementById('zoom-overlay').classList.add('active');
}

document.getElementById('close-zoom').addEventListener('click', () => {
  document.getElementById('zoom-overlay').classList.remove('active');
});

 // Testimonial images with links
  const testimonialImages = [
    {
      src: "images/testimonials/T1.png",
      link: "https://www.facebook.com/share/r/15VAmxhF1q/"
    },
    {
      src: "images/testimonials/T2.png",
      link: "https://www.facebook.com/darlingsfinejewels/reviews"
    },
    {
      src: "images/testimonials/T3.png",
      link: "https://www.facebook.com/darlingsfinejewels/reviews"
    },
    {
      src: "images/testimonials/T4.png",
      link: "https://www.facebook.com/darlingsfinejewels/reviews"
    }
  ];

  // Render testimonial images in the container
  function renderPhotoTestimonials() {
    const container = document.getElementById("testimonial-photo-container");

    if (!container) {
      console.error("❌ Testimonial container not found!");
      return;
    }

    testimonialImages.forEach((item, index) => {
      const isEven = index % 2 === 0;
      const wrapper = document.createElement("div");
      wrapper.className = `testimonial-photo-wrapper ${isEven ? 'left' : 'right'}`;

      wrapper.innerHTML = `
        <a href="${item.link}" target="_blank" class="testimonial-card">
          <img src="${item.src}" alt="Testimonial ${index + 1}">
        </a>
      `;

      container.appendChild(wrapper);
    });
  }

  // Load the testimonials once the DOM is ready
  window.addEventListener('load', () => {
    renderPhotoTestimonials();
  });
