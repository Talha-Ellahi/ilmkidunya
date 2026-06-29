//let scriptLoaded = !1; function loadShareThisScript() { if (!scriptLoaded) { const t = document.createElement("script"); t.id = "sharethis", t.type = "text/javascript", t.src = "https://platform-api.sharethis.com/js/sharethis.js#property=64e86dff0ba20000199f75ba&product=sop", t.async = !0, t.defer = !0, t.onload = function () { window.__sharethis__ && window.__sharethis__.load("inline-share-buttons", "64e86dff0ba20000199f75ba") }, document.body.appendChild(t), scriptLoaded = !0, window.removeEventListener("scroll", loadShareThisScript) } } async function showLoginPopup(t) { if (!document.querySelector(".login-sigin-wraper")) { const e = await fetch(`/account/LoadLoginPartial?returnUrl=${encodeURIComponent(t)}`), o = await e.text(); document.getElementById("loginPartialContainer").innerHTML = o, initLoginPopupEvents() } const e = document.querySelector(".login-sigin-wraper"), o = document.getElementById("pop-close-btn"); e.classList.add("visible"), o && (o.onclick = () => e.classList.remove("visible")) } function initLoginPopupEvents() { document.querySelectorAll(".login-google-btn").forEach((t => { t.addEventListener("click", (function (t) { t.preventDefault(), window.open(this.href, "GoogleLogin", "width=500,height=600") })) })), document.querySelectorAll(".login-fb-btn").forEach((t => { t.addEventListener("click", (function (t) { t.preventDefault(), window.open(this.href, "FbLogin", "width=500,height=600") })) })) } async function getfeedback() { const t = document.querySelector("#Comments").value.trim(), e = window.location.pathname; if (t) try { const o = await fetch("/savepage-feedback", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ Comments: t, PageUrl: e }) }); await o.json() && alert("✅ Feedback submitted successfully"), document.querySelector("#Comments").value = "", document.querySelector("#Comments").placeholder = "Please write your feedback about this page here." } catch (t) { console.error(t), alert("❌ Error while saving feedback") } else alert("Please enter feedback") } async function logoutUser() { try { const t = await fetch("/Account/Logout", { method: "POST", headers: { "X-Requested-With": "XMLHttpRequest", "Content-Type": "application/json" } }); if ((await t.json()).success) { const t = await (await fetch("/Home/NavbarPartial")).text(); document.querySelector(".nav-item.dropdown").outerHTML = t; const e = document.querySelector(".nav-item a[aria-label='link']"); e && e.addEventListener("click", (() => showLoginPopup(window.location.href))) } } catch (t) { console.error("Logout failed:", t) } } window.addEventListener("scroll", loadShareThisScript), window.addEventListener("message", (function (t) { if (t.data && t.data.success) { alert(`✅ ${t.data.message}\nWelcome ${t.data.user.name}`); const e = document.querySelector(".nav-item a[aria-label='link']"); e && (e.outerHTML = `\n\t\t\t\t\t\t<a class="nav-link dropdown-toggle" id="profileDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">\n\t\t\t\t\t\t\t<img src="${t.data.user.picture}" alt="profile" style="border-radius:50%; width:32px; height:32px;" />\n\t\t\t\t\t\t</a>\n\t\t\t\t\t\t<ul class="dropdown-menu dropdown-menu-end" aria-labelledby="profileDropdown">\n\t\t\t\t\t\t\t<li>\n\t\t\t\t\t\t\t\t<form action="/account/logout" method="post">\n\t\t\t\t\t\t\t\t\t<button type="submit" class="dropdown-item">Logout</button>\n\t\t\t\t\t\t\t\t</form>\n\t\t\t\t\t\t\t</li>\n\t\t\t\t\t\t</ul>`); const o = document.querySelector(".login-sigin-wraper"); o && (o.style.display = "none"), "function" == typeof onLoginSuccess && onLoginSuccess(t.data.user) } })), "#_=_" === window.location.hash && (history.replaceState ? history.replaceState(null, null, window.location.href.split("#")[0]) : window.location.hash = ""), window.addEventListener("message", (async t => { if (t.data && t.data.success) { const e = await (await fetch("/Home/NavbarPartial")).text(); document.querySelector(".nav-item.dropdown").outerHTML = e; const o = document.querySelector(".login-sigin-wraper"); o && (o.style.display = "none"), "function" == typeof onLoginSuccess && onLoginSuccess(t.data.user) } })), window.addEventListener("DOMContentLoaded", (() => { document.querySelectorAll(".btnLoadMore").forEach((t => { t.addEventListener("click", (() => { const e = t.querySelector("span.icon"), o = t.querySelector("i"); "View less detail" === o.textContent.trim() ? (o.textContent = "View More detail", e.textContent = "+") : (o.textContent = "View less detail", e.textContent = "-"), document.querySelector(".load-more-content").classList.toggle("show-more-height") })) })) }));


let scriptLoaded = false;

const SHARETHIS_PROPERTY_ID = "64e86dff0ba20000199f75ba";

function createShareThisContainer() {
    const shareBox = document.querySelector(".share-box");

    if (!shareBox) return false;

    if (!shareBox.querySelector(".sharethis-inline-share-buttons")) {
        const div = document.createElement("div");
        div.className = "sharethis-inline-share-buttons";
        div.style.marginBottom = "20px";
        shareBox.appendChild(div);
    }

    return true;
}

function initShareThis(retry = 0) {
    const containerReady = createShareThisContainer();

    if (!containerReady) {
        if (retry < 20) {
            setTimeout(() => initShareThis(retry + 1), 300);
        }
        return;
    }

    if (
        window.__sharethis__ &&
        typeof window.__sharethis__.initialize === "function"
    ) {
        setTimeout(() => {
            window.__sharethis__.initialize();
        }, 300);
        return;
    }

    if (retry < 20) {
        setTimeout(() => initShareThis(retry + 1), 300);
    }
}

function loadShareThisScript() {
    createShareThisContainer();

    if (scriptLoaded || document.getElementById("sharethis")) {
        initShareThis();
        return;
    }

    scriptLoaded = true;

    const script = document.createElement("script");
    script.id = "sharethis";
    script.type = "text/javascript";
    script.src =
        "https://platform-api.sharethis.com/js/sharethis.js#property=" +
        SHARETHIS_PROPERTY_ID +
        "&product=inline-share-buttons";

    script.async = true;
    script.defer = true;

    script.onload = () => {
        initShareThis();
    };

    script.onerror = () => {
        console.error("ShareThis script failed to load");
        scriptLoaded = false;
    };

    document.body.appendChild(script);
}

function lazyLoadShareThis() {
    loadShareThisScript();

    window.removeEventListener("scroll", lazyLoadShareThis);
    window.removeEventListener("click", lazyLoadShareThis);
    window.removeEventListener("mousemove", lazyLoadShareThis);
    window.removeEventListener("touchstart", lazyLoadShareThis);
}

async function showLoginPopup(t) {
    if (!document.querySelector(".login-sigin-wraper")) {
        const e = await fetch(
            `/account/LoadLoginPartial?returnUrl=${encodeURIComponent(t)}`
        );
        const o = await e.text();

        document.getElementById("loginPartialContainer").innerHTML = o;
        initLoginPopupEvents();
    }

    const e = document.querySelector(".login-sigin-wraper");
    const o = document.getElementById("pop-close-btn");

    e.classList.add("visible");

    if (o) {
        o.onclick = () => e.classList.remove("visible");
    }
}

function initLoginPopupEvents() {
    document.querySelectorAll(".login-google-btn").forEach((t) => {
        t.addEventListener("click", function (t) {
            t.preventDefault();
            window.open(this.href, "GoogleLogin", "width=500,height=600");
        });
    });

    document.querySelectorAll(".login-fb-btn").forEach((t) => {
        t.addEventListener("click", function (t) {
            t.preventDefault();
            window.open(this.href, "FbLogin", "width=500,height=600");
        });
    });
}

async function getfeedback() {
    const t = document.querySelector("#Comments").value.trim();
    const e = window.location.pathname;

    if (!t) {
        alert("Please enter feedback");
        return;
    }

    try {
        const o = await fetch("/savepage-feedback", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                Comments: t,
                PageUrl: e,
            }),
        });

        await o.json();

        alert("✅ Feedback submitted successfully");

        document.querySelector("#Comments").value = "";
        document.querySelector("#Comments").placeholder =
            "Please write your feedback about this page here.";
    } catch (t) {
        console.error(t);
        alert("❌ Error while saving feedback");
    }
}

async function logoutUser() {
    try {
        const t = await fetch("/Account/Logout", {
            method: "POST",
            headers: {
                "X-Requested-With": "XMLHttpRequest",
                "Content-Type": "application/json",
            },
        });

        const result = await t.json();

        if (result.success) {
            const navbarHtml = await (
                await fetch("/Home/NavbarPartial")
            ).text();

            const navDropdown = document.querySelector(".nav-item.dropdown");

            if (navDropdown) {
                navDropdown.outerHTML = navbarHtml;
            }

            const loginLink = document.querySelector(
                ".nav-item a[aria-label='link']"
            );

            if (loginLink) {
                loginLink.addEventListener("click", () =>
                    showLoginPopup(window.location.href)
                );
            }
        }
    } catch (t) {
        console.error("Logout failed:", t);
    }
}

function handleLoginSuccess(t) {
    if (t.data && t.data.success) {
        const e = document.querySelector(".nav-item a[aria-label='link']");

        if (e) {
            e.outerHTML = `
                <a class="nav-link dropdown-toggle" id="profileDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <img src="${t.data.user.picture}" alt="profile" style="border-radius:50%; width:32px; height:32px;" />
                </a>
                <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="profileDropdown">
                    <li>
                        <form action="/account/logout" method="post">
                            <button type="submit" class="dropdown-item">Logout</button>
                        </form>
                    </li>
                </ul>`;
        }

        const o = document.querySelector(".login-sigin-wraper");

        if (o) {
            o.style.display = "none";
        }

        if (typeof onLoginSuccess === "function") {
            onLoginSuccess(t.data.user);
        }
    }
}

async function handleNavbarRefreshAfterLogin(t) {
    if (t.data && t.data.success) {
        const e = await (await fetch("/Home/NavbarPartial")).text();

        const navDropdown = document.querySelector(".nav-item.dropdown");

        if (navDropdown) {
            navDropdown.outerHTML = e;
        }

        const o = document.querySelector(".login-sigin-wraper");

        if (o) {
            o.style.display = "none";
        }

        if (typeof onLoginSuccess === "function") {
            onLoginSuccess(t.data.user);
        }
    }
}

window.addEventListener("DOMContentLoaded", function () {
    createShareThisContainer();

    window.addEventListener("scroll", lazyLoadShareThis, {
        passive: true,
        once: true,
    });

    window.addEventListener("click", lazyLoadShareThis, {
        once: true,
    });

    window.addEventListener("mousemove", lazyLoadShareThis, {
        passive: true,
        once: true,
    });

    window.addEventListener("touchstart", lazyLoadShareThis, {
        passive: true,
        once: true,
    });

    document.querySelectorAll(".btnLoadMore").forEach((t) => {
        t.addEventListener("click", () => {
            const e = t.querySelector("span.icon");
            const o = t.querySelector("i");

            if (!e || !o) return;

            if (o.textContent.trim() === "View less detail") {
                o.textContent = "View More detail";
                e.textContent = "+";
            } else {
                o.textContent = "View less detail";
                e.textContent = "-";
            }

            const loadMoreContent = document.querySelector(".load-more-content");

            if (loadMoreContent) {
                loadMoreContent.classList.toggle("show-more-height");
            }
        });
    });
});

window.addEventListener("message", handleLoginSuccess);
window.addEventListener("message", handleNavbarRefreshAfterLogin);

if (window.location.hash === "#_=_") {
    if (history.replaceState) {
        history.replaceState(null, null, window.location.href.split("#")[0]);
    } else {
        window.location.hash = "";
    }
}