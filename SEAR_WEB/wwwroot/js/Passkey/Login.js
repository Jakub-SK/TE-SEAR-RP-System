async function loginPasskey() {
    document.getElementById("loadingMessage").innerText = "Loading...";

    try {
        const response = await fetch('/Passkey/LoginRequest', {
            method: 'POST'
        });

        if (!response.ok) {
            alert("Login request failed.");
            return;
        }

        const options = await response.json();

        options.challenge = base64UrlToBuffer(options.challenge);

        if (options.allowCredentials) {
            options.allowCredentials.forEach(x => {
                x.id = base64UrlToBuffer(x.id);
            });
        }

        const assertion = await navigator.credentials.get({
            publicKey: options
        });

        const result = await fetch('/Passkey/LoginResponse', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(assertion)
        });

        if (!result.ok) {
            alert("Passkey Credentials are incorrect.");
            return;
        }

        const responseJson = await result.json();

        if (responseJson.success) {
            window.location.href = responseJson.redirectUrl;
        } else {
            alert("Login failed.");
        }
    } catch {
        alert("Unexpected error occurred.");
    } finally {
        document.getElementById("loadingMessage").innerText = "";
    }
}
