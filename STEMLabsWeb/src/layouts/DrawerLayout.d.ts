import { type JSX } from "react";
export type DrawerListElement = {
    icon: JSX.Element;
    text: string;
    link: string;
};
export declare function getDrawerList(role: string | undefined): DrawerListElement[];
export default function DrawerLayout(): import("react/jsx-runtime").JSX.Element;
