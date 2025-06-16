import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useContext, useState } from "react";
import { ToastContext } from "../../../layouts/ToastLayout.tsx";
import { AuthContext } from "../../../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../../../helpers/ToastErrorMessageHandle.tsx";
import { Box, Button, CircularProgress, TextField, Typography, } from "@mui/material";
import { axiosRequestWithAutoReauth } from "../../../helpers/axiosRequestWithAutoReauth.tsx";
export default function GenerateRecoveryCodeSection({ setUsernameParent, }) {
    const [username, setUsername] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { addToast } = useContext(ToastContext);
    const { setUser } = useContext(AuthContext);
    function handleUsernameChange(event) {
        setUsername(event.target.value);
    }
    function handleSubmit() {
        setIsSubmitting(true);
        axiosRequestWithAutoReauth({
            method: "POST",
            url: `${import.meta.env.VITE_API_URL}/api/recovery/password-request`,
            data: username,
            headers: { "Content-Type": "application/json" },
        }, setUser)
            .then(() => {
            setUsernameParent(username);
        })
            .catch((error) => {
            toastErrorMessageHandle(addToast, setUser, error);
        })
            .finally(() => {
            setIsSubmitting(false);
        });
    }
    return (_jsxs(Box, { display: "flex", flexDirection: "column", gap: 2, padding: 2, children: [_jsx(Typography, { variant: "h4", align: "center", marginBottom: 5, children: "Enter your username to recover your password." }), _jsx(TextField, { label: "Username", value: username, onChange: handleUsernameChange }), _jsx(Button, { disabled: isSubmitting, variant: "contained", size: "large", onClick: handleSubmit, children: isSubmitting ? _jsx(CircularProgress, {}) : "Next" })] }));
}
