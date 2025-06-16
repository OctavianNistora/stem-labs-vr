import { jsx as _jsx } from "react/jsx-runtime";
import { Box } from "@mui/material";
import { Outlet } from "react-router";
export default function AuthLayout() {
    return (_jsx(Box, { display: "flex", width: "100%", height: "100%", bgcolor: "#F4F4F4", children: _jsx(Box, { width: "500px", height: "500px", bgcolor: "white", display: "flex", flexDirection: "column", margin: "auto", alignItems: "center", justifyContent: "center", boxShadow: 10, gap: 2, padding: 2, children: _jsx(Outlet, {}) }) }));
}
