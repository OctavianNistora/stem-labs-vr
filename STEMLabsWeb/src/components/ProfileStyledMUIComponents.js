import { jsx as _jsx } from "react/jsx-runtime";
import { ListItemIcon, ListItemText, styled, TextField, } from "@mui/material";
export const ProfileStyledListItemIcon = styled(ListItemIcon)(({}) => ({
    "& svg": {
        fontSize: "2em",
    },
}));
export const ProfileStyledListItemText = styled(ListItemText)(({}) => ({
    "& .MuiTypography-root": {
        fontSize: "1.2em",
    },
}));
export const ProfileStyledTextField = (props) => {
    const { readOnly, ...rest } = props;
    return (_jsx(TextField, { variant: "standard", slotProps: readOnly
            ? {
                input: {
                    readOnly: true,
                    disableUnderline: true,
                },
                inputLabel: {
                    shrink: true,
                },
            }
            : undefined, ...rest }));
};
