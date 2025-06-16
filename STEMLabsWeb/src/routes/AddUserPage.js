import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, Button, FormControl, InputLabel, MenuItem, Select, TextField, } from "@mui/material";
import { useContext, useState } from "react";
import { isMobilePhone } from "validator";
import { useNavigate } from "react-router";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import isEmail from "validator/lib/isEmail";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";
const Options = [
    { value: "student", label: "Student" },
    { value: "professor", label: "Professor" },
];
export default function AddUserPage() {
    const [email, setEmail] = useState("");
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [role, setRole] = useState(Options[0].value);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const navigate = useNavigate();
    const { addToast } = useContext(ToastContext);
    const { user, setUser } = useContext(AuthContext);
    function handleEmailChange(event) {
        setEmail(event.target.value);
    }
    function handleFirstNameChange(event) {
        setFirstName(event.target.value);
    }
    function handleLastNameChange(event) {
        setLastName(event.target.value);
    }
    function handlePhoneNumberChange(event) {
        setPhoneNumber(event.target.value);
    }
    function handleRoleChange(event) {
        setRole(event.target.value);
    }
    function handleSubmit() {
        const data = {
            email: email,
            firstName: firstName,
            lastName: lastName,
            phoneNumber: phoneNumber,
            role: role,
        };
        setIsSubmitting(true);
        axiosRequestWithAutoReauth({
            method: "POST",
            url: `${import.meta.env.VITE_API_URL}/api/users`,
            data: data,
            headers: {
                Authorization: `Bearer ${user?.accessToken}`,
            },
        }, setUser)
            .then((_) => {
            addToast({
                message: "User added successfully",
                variant: "success",
            });
            navigate("/manage-users");
        })
            .catch((error) => {
            toastErrorMessageHandle(addToast, setUser, error);
            setIsSubmitting(false);
        });
    }
    const isValidEmail = !email || isEmail(email);
    const isValidPhoneNumber = !phoneNumber || isMobilePhone(phoneNumber);
    return (_jsxs(Box, { display: "flex", flexDirection: "column", width: "400px", height: "500px", color: "white", boxShadow: 4, justifyContent: "center", alignItems: "center", gap: 2, children: [_jsx(TextField, { required: true, label: "Email Address", value: email, error: !isValidEmail, helperText: !isValidEmail ? "Invalid email address" : undefined, onChange: handleEmailChange }), _jsx(TextField, { label: "First Name", value: firstName, onChange: handleFirstNameChange }), _jsx(TextField, { label: "Last Name", value: lastName, onChange: handleLastNameChange }), _jsx(TextField, { label: "Phone Number", error: !isValidPhoneNumber, helperText: !isValidPhoneNumber ? "Invalid phone number" : undefined, value: phoneNumber, onChange: handlePhoneNumberChange }), _jsxs(FormControl, { sx: { width: "150px" }, children: [_jsx(InputLabel, { children: "Role" }), _jsx(Select, { required: true, label: "Role", value: role, onChange: (event) => handleRoleChange(event), children: Options.map((option) => (_jsx(MenuItem, { value: option.value, children: option.label }, option.value))) })] }), _jsx(Button, { disabled: !(!!email && !!role && isValidEmail && isValidPhoneNumber) ||
                    isSubmitting, variant: "contained", color: "primary", onClick: handleSubmit, children: "Add User" })] }));
}
