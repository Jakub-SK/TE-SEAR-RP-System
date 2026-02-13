async function registerAdditionalPasskey() {
    document.getElementById("loadingMessage").innerText = "Loading...";
    const errorBox = document.getElementById("errorMessage");
    errorBox.innerText = "";
    const keyId = document.getElementById('keyId').value;

    try {
        const response = await fetch('/Passkey/ConfirmRegisterAdditionalPasskey', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                KeyId: keyId
            })
        });

        if (!response.ok) {
            errorBox.innerText = "User creation or request failed.";
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

        const verifyResponse = await fetch('/Passkey/RegisterResponseByUserId', {
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
        errorBox.innerText = "Unexpected error occurred.";
    } finally {
        document.getElementById("loadingMessage").innerText = "";
    }
}