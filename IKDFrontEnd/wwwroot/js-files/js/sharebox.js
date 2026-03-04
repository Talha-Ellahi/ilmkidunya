
	let scriptLoaded = false;

	function loadShareThisScript() {
				if (!scriptLoaded) {
					const script = document.createElement("script");
	script.id = "sharethis";
	script.type = "text/javascript";
	script.src = "https://platform-api.sharethis.com/js/sharethis.js#property=64e86dff0ba20000199f75ba&product=sop";
	script.async = true;
	script.defer = true;

	script.onload = function () {
						// Jab script load ho jaye tab ShareThis initialize kare
						if (window.__sharethis__) {
		window.__sharethis__.load('inline-share-buttons', '64e86dff0ba20000199f75ba');
						}
					};

	document.body.appendChild(script);
	scriptLoaded = true;
	window.removeEventListener("scroll", loadShareThisScript); // dobara load na ho
				}
			}

	// Jab user scroll kare tabhi script load hogi
	window.addEventListener("scroll", loadShareThisScript);
