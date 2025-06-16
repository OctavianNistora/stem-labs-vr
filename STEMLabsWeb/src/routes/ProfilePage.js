import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, Drawer, List, ListItem, ListItemButton } from "@mui/material";
import PersonIcon from "@mui/icons-material/Person";
import EmailIcon from "@mui/icons-material/Email";
import PasswordIcon from "@mui/icons-material/Password";
import { useState } from "react";
import { ProfileStyledListItemIcon, ProfileStyledListItemText, } from "../components/ProfileStyledMUIComponents.tsx";
import ProfilePageProfileSection from "../components/page-sections/profile-page/ProfilePageProfileSection.tsx";
import ProfilePageEmailSection from "../components/page-sections/profile-page/ProfilePageEmailSection.tsx";
import ProfilePagePasswordSection from "../components/page-sections/profile-page/ProfilePagePasswordSection.tsx";
const drawerWidth = 200;
export default function ProfilePage() {
    const [pageSection, setPageSection] = useState("profile");
    function handleSectionChange(section) {
        setPageSection(section);
    }
    function sectionContent(pageSection) {
        switch (pageSection) {
            case "profile":
                return _jsx(ProfilePageProfileSection, {});
            case "email":
                return _jsx(ProfilePageEmailSection, {});
            case "password":
                return _jsx(ProfilePagePasswordSection, {});
            default:
                return _jsx(ProfilePageProfileSection, {});
        }
    }
    return (_jsxs(Box, { id: "profile-container", display: "flex", bgcolor: "white", padding: 2, width: "35%", height: "50%", position: "relative", boxShadow: 3, children: [_jsx(Drawer, { variant: "permanent", slotProps: {
                    paper: { sx: { width: drawerWidth, position: "absolute" } },
                }, sx: { width: drawerWidth }, children: _jsxs(List, { sx: { fontSize: 12 }, children: [_jsx(ListItem, { disablePadding: true, children: _jsxs(ListItemButton, { onClick: () => handleSectionChange("profile"), children: [_jsx(ProfileStyledListItemIcon, { children: _jsx(PersonIcon, {}) }), _jsx(ProfileStyledListItemText, { primary: _jsx(Box, { fontWeight: pageSection === "profile" ? "bold" : "normal", children: "Profile" }) })] }) }, "profile"), _jsx(ListItem, { disablePadding: true, children: _jsxs(ListItemButton, { onClick: () => handleSectionChange("email"), children: [_jsx(ProfileStyledListItemIcon, { children: _jsx(EmailIcon, {}) }), _jsx(ProfileStyledListItemText, { primary: _jsx(Box, { fontWeight: pageSection === "email" ? "bold" : "normal", children: "Email" }) })] }) }, "email"), _jsx(ListItem, { disablePadding: true, children: _jsxs(ListItemButton, { onClick: () => handleSectionChange("password"), children: [_jsx(ProfileStyledListItemIcon, { children: _jsx(PasswordIcon, {}) }), _jsx(ProfileStyledListItemText, { primary: _jsx(Box, { fontWeight: pageSection === "password" ? "bold" : "normal", children: "Password" }) })] }) }, "password")] }) }), _jsx(Box, { display: "flex", width: "100%", height: "100%", justifyContent: "center", alignItems: "center", children: sectionContent(pageSection) })] }));
}
