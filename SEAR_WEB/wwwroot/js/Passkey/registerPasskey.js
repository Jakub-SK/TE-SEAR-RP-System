async function registerPasskey() {
    document.getElementById("loadingMessage").innerText = "Loading...";
    const username = document.getElementById("username").value.trim();
    const displayName = document.getElementById("displayName").value.trim();
    const errorBox = document.getElementById("errorMessage");
    errorBox.innerText = "";

    if (!username || !displayName) {
        errorBox.innerText = "Username and Display Name are required.";
        return;
    }

    try {
        const response = await fetch('/Passkey/RegisterRequest', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, displayName })
        });

        if (!response.ok) {
            errorBox.innerText = "Register failed, username maybe already been used.";
            return;
        }

        const options = await response.json();

        options.challenge = base64UrlToBuffer(options.challenge);
        options.user.id = base64UrlToBuffer(options.user.id);

        if (options.excludeCredentials) {
            options.excludeCredentials.forEach(x => {
                x.id = base64UrlToBuffer(x.id);
            });
        }

        const credential = await navigator.credentials.create({
            publicKey: options
        });

        const verifyResponse = await fetch('/Passkey/RegisterResponse', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(credential)
        });

        if (!verifyResponse.ok) {
            errorBox.innerText = "Passkey registration failed.";
            return;
        }

        const responseJson = await verifyResponse.json();

        if (responseJson.success) {
            window.location.href = responseJson.redirectUrl;
        } else {
            alert("Passkey registration failed.");
        }
    } catch {
        const response = await fetch('/Passkey/RemoveUserByUsername', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username })
        });

        if (!response.ok) {
            errorBox.innerText = "Unable to remove user account";
            return;
        }

        errorBox.innerText = "Unexpected error occurred.";
    } finally {
        document.getElementById("loadingMessage").innerText = "";
    }
}