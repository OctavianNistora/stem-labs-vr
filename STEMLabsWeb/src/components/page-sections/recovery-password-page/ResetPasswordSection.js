import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useContext, useState } from "react";
import { useNavigate } from "react-router";
import { ToastContext } from "../../../layouts/ToastLayout.tsx";
import { AuthContext } from "../../../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../../../helpers/ToastErrorMessageHandle.tsx";
import { Box, Button, CircularProgress, TextField, Typography, } from "@mui/material";
import { axiosRequestWithAutoReauth } from "../../../helpers/axiosRequestWithAutoReauth.tsx";
export default function ResetPasswordSection({ username, }) {
    const [newPassword, setNewPassword] = useState("");
    const [newPasswordConfirmation, setNewPasswordConfirmation] = useState("");
    const [token, setToken] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const navigate = useNavigate();
    const { addToast } = useContext(ToastContext);
    const { setUser } = useContext(AuthContext);
    function handlePasswordChange(event) {
        setNewPassword(event.target.value);
    }
    function handlePasswordConfirmationChange(event) {
        setNewPasswordConfirmation(event.target.value);
    }
    function handleTokenChange(event) {
        setToken(event.target.value);
    }
    function handleSubmit() {
        const data = {
            username: username,
            newPassword: newPassword,
            token: token,
        };
        setIsSubmitting(true);
        axiosRequestWithAutoReauth({
            method: "POST",
            url: `${import.meta.env.VITE_API_URL}/api/recovery/password-reset`,
            data: data,
            headers: { "Content-Type": "application/json" },
        }, setUser)
            .then(() => {
            addToast({
                message: "Password reset successfully.",
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
    const doPasswordsMatch = !newPassword ||
        !newPasswordConfirmation ||
        newPassword == newPasswordConfirmation;
    return (_jsxs(Box, { display: "flex", flexDirection: "column", gap: 2, padding: 2, children: [_jsx(Typography, { variant: "h4", align: "center", marginBottom: 5, children: "Reset your password." }), _jsx(TextField, { label: "New Password", type: "password", value: newPassword, onChange: handlePasswordChange }), _jsx(TextField, { label: "Confirm New Password", type: "password", error: !doPasswordsMatch, helperText: !doPasswordsMatch ? "Passwords do not match." : undefined, value: newPasswordConfirmation, onChange: handlePasswordConfirmationChange }), _jsx(TextField, { label: "Recovery Code", value: token, onChange: handleTokenChange }), _jsx(Button, { disabled: isSubmitting, variant: "contained", size: "large", onClick: handleSubmit, children: isSubmitting ? _jsx(CircularProgress, {}) : "Reset Password" })] }));
}
