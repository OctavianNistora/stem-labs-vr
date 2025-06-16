import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, Button, CircularProgress, TextField, Typography, } from "@mui/material";
import { useContext, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { useNavigate } from "react-router";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
export default function RecoveryUsernamePage() {
    const [email, setEmail] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const navigate = useNavigate();
    const { addToast } = useContext(ToastContext);
    const { setUser } = useContext(AuthContext);
    function handleEmailChange(event) {
        setEmail(event.target.value);
    }
    function handleSubmit() {
        setIsSubmitting(true);
        axiosRequestWithAutoReauth({
            method: "post",
            url: `${import.meta.env.VITE_API_URL}/api/recovery/username-reminder`,
            data: email,
            headers: {
                "Content-Type": "application/json",
            },
        }, setUser)
            .then(() => {
            addToast({
                message: "A recovery email has been sent to your email address.",
                variant: "success",
            });
            navigate("/login");
        })
            .catch((error) => {
            toastErrorMessageHandle(addToast, setUser, error);
        })
            .finally(() => {
            setIsSubmitting(false);
        });
    }
    return (_jsxs(Box, { display: "flex", flexDirection: "column", gap: 2, padding: 2, children: [_jsx(Typography, { variant: "h4", align: "center", marginBottom: 5, children: "Enter your email address to recover your username." }), _jsx(TextField, { label: "Email", value: email, onChange: handleEmailChange }), _jsx(Button, { disabled: isSubmitting, variant: "contained", size: "large", onClick: handleSubmit, children: isSubmitting ? _jsx(CircularProgress, {}) : "Send Recovery Email" })] }));
}
