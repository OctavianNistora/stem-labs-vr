import { jsx as _jsx, Fragment as _Fragment, jsxs as _jsxs } from "react/jsx-runtime";
import { Button } from "@mui/material";
import { Link } from "react-router";
export default function RecoverySelectionPage() {
    return (_jsxs(_Fragment, { children: [_jsx(Button, { variant: "contained", size: "large", component: Link, to: "username", children: "Forgot Username" }), _jsx(Button, { variant: "contained", size: "large", component: Link, to: "password", children: "Forgot Password" })] }));
}
