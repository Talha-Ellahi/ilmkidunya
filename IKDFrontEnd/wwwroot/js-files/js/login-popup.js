
	async function showLoginPopup(returnUrl) {
			// Agar pehle se loaded nahi hai to fetch karo
			if (!document.querySelector('.login-sigin-wraper')) {
				const res = await fetch(`/account/LoadLoginPartial?returnUrl=${encodeURIComponent(returnUrl)}`);
	const html = await res.text();
	document.getElementById("loginPartialContainer").innerHTML = html;

	// ✅ Bind buttons after injecting partial
	initLoginPopupEvents();
			}

	const loginPopup = document.querySelector('.login-sigin-wraper');
	const popCloseBtn = document.getElementById('pop-close-btn');

	// show popup
	loginPopup.classList.add('visible');

	// close button
	if (popCloseBtn) {
		popCloseBtn.onclick = () => loginPopup.classList.remove('visible');
			}
		}

	function initLoginPopupEvents() {

		document.querySelectorAll(".login-google-btn").forEach(btn => {
			btn.addEventListener("click", function (e) {
				e.preventDefault();
				window.open(this.href, "GoogleLogin", "width=500,height=600");
			});
		});

			document.querySelectorAll(".login-fb-btn").forEach(btn => {
		btn.addEventListener("click", function (e) {
			e.preventDefault();
			window.open(this.href, "FbLogin", "width=500,height=600");
		});
			});
		}

	// ✅ Listen for login success from popup
	window.addEventListener("message", function (event) {
			if (event.data && event.data.success) {
		alert(`✅ ${event.data.message}\nWelcome ${event.data.user.name}`);

	// Example: update navbar profile icon
	const loginIcon = document.querySelector(".nav-item a[aria-label='link']");
	if (loginIcon) {
		loginIcon.outerHTML = `
						<a class="nav-link dropdown-toggle" id="profileDropdown" role="button" aria-expanded="false">
							<img src="${event.data.user.picture}" alt="profile" style="border-radius:50%; width:32px; height:32px;" />
						</a>
						<ul class="dropdown-menu dropdown-menu-end" aria-labelledby="profileDropdown">
							<li>
								<form action="/account/logout" method="post">
									<button type="submit" class="dropdown-item">Logout</button>
								</form>
							</li>
						</ul>`;
				}
				setTimeout(() => {
					const ikdProfileTrigger = document.getElementById("profileDropdown");
					const ikdProfileMenu = document.querySelector(".dropdown-menu");

					if (ikdProfileTrigger && ikdProfileMenu) {

						// Default hide
						ikdProfileMenu.style.display = "none";

						// Hover in → show
						ikdProfileTrigger.addEventListener("mouseenter", () => {
							ikdProfileMenu.style.display = "block";
						});

						// Hover out → hide
						ikdProfileTrigger.addEventListener("mouseleave", () => {
							ikdProfileMenu.style.display = "none";
						});

						// Hover on menu → keep open
						ikdProfileMenu.addEventListener("mouseenter", () => {
							ikdProfileMenu.style.display = "block";
						});

						// Leave menu → hide
						ikdProfileMenu.addEventListener("mouseleave", () => {
							ikdProfileMenu.style.display = "none";
						});
					}
				}, 500);
	// hide popup
	const loginPopup = document.querySelector(".login-sigin-wraper");
	if (loginPopup) loginPopup.style.display = "none";

	// optional callback
	if (typeof onLoginSuccess === "function") {
		onLoginSuccess(event.data.user);
				}
			}
		});
