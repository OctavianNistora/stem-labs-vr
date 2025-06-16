import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Stack, Typography, } from "@mui/material";
import { getDrawerList } from "../layouts/DrawerLayout";
import { useContext } from "react";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { Link } from "react-router";
export default function HomePage() {
    const { user } = useContext(AuthContext);
    return (_jsxs(Box, { width: "100%", height: "100%", display: "flex", flexDirection: "column", justifyContent: "center", alignContent: "center", alignItems: "center", gap: 5, children: [_jsx(Typography, { variant: "h3", sx: { fontWeight: "bold" }, children: "Welcome to STEM Labs VR" }), _jsx(List, { component: Stack, direction: "row", children: getDrawerList(user?.role).map((item) => (_jsx(ListItem, { alignItems: "flex-start", children: _jsx(ListItemButton, { id: `${item.text.toLowerCase().replace(" ", "-")}-homepage-button`, component: Link, to: item.link, children: _jsxs(Stack, { alignItems: "center", alignContent: "center", children: [_jsx(ListItemIcon, { sx: {
                                        "& svg": {
                                            fontSize: "6em",
                                            bgcolor: "white",
                                            borderRadius: "10%",
                                            boxShadow: 2,
                                        },
                                    }, children: item.icon }), _jsx(ListItemText, { slotProps: { primary: { fontSize: "1em" } }, primary: _jsx(Box, { textAlign: "center", children: item.text }) })] }) }) }, item.text))) })] }));
}
