import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { Box, Button, CircularProgress, TextField, Typography, } from "@mui/material";
import { useContext, useState } from "react";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { Link, useNavigate } from "react-router";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
export default function LoginPage() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [isAuthenticating, setIsAuthenticating] = useState(false);
    const { addToast } = useContext(ToastContext);
    const { setUser } = useContext(AuthContext);
    const navigate = useNavigate();
    function handleSubmit(_) {
        const data = {
            username: username,
            password: password,
            respondWithRefreshToken: true,
        };
        setIsAuthenticating(true);
        axiosRequestWithAutoReauth({
            method: "POST",
            url: `${import.meta.env.VITE_API_URL}/api/auth/session`,
            data: data,
        }, setUser)
            .then((res) => {
            const { uid, accessToken, refreshToken, role } = res.data;
            localStorage.setItem("refreshToken", refreshToken);
            setUser({ uid, accessToken, role });
            navigate("/");
        })
            .catch((error) => {
            toastErrorMessageHandle(addToast, setUser, error);
        })
            .finally(() => {
            setIsAuthenticating(false);
        });
    }
    return (_jsxs(_Fragment, { children: [_jsxs(Box, { padding: 2, justifyItems: "center", children: [_jsx(Typography, { variant: "h3", children: "STEM Labs VR" }), _jsx(Typography, { variant: "h5", children: "Authentication" })] }), _jsxs(Box, { display: "flex", width: "80%", flexDirection: "column", gap: 2, children: [_jsx(TextField, { id: "username-field", fullWidth: true, type: "text", autoComplete: "username", label: "Username", value: username, onChange: (e) => setUsername(e.currentTarget.value) }), _jsx(TextField, { id: "password-field", fullWidth: true, type: "password", autoComplete: "current-password", label: "Password", value: password, onChange: (e) => setPassword(e.currentTarget.value) }), _jsx(Button, { id: "login-button", disabled: isAuthenticating, fullWidth: true, size: "large", variant: "contained", onClick: handleSubmit, children: isAuthenticating ? _jsx(CircularProgress, {}) : "Log in" }), _jsx(Button, { id: "recovery-button", disabled: isAuthenticating, fullWidth: true, size: "small", component: Link, to: "/recovery", children: "Cannot log in" })] })] }));
}
