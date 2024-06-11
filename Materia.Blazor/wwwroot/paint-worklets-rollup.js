/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
/**
 * Paints cut corners.
 */
class CutCornerShapePainter {
    constructor() {
        this.bleed = 1;
    }
    paintStartStart(ctx) {
        ctx.lineTo(1, 0);
    }
    paintStartEnd(ctx) {
        ctx.lineTo(1, 1);
    }
    paintEndStart(ctx) {
        ctx.lineTo(0, 0);
    }
    paintEndEnd(ctx) {
        ctx.lineTo(0, 1);
    }
}

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
/**
 * Paints rounded corners.
 */
class RoundedCornerShapePainter {
    constructor() {
        this.bleed = 1;
    }
    paintStartStart(ctx) {
        ctx.arc(1, 1, 1, Math.PI, 3 * Math.PI / 2);
    }
    paintStartEnd(ctx) {
        ctx.arc(0, 1, 1, 3 * Math.PI / 2, 2 * Math.PI);
    }
    paintEndStart(ctx) {
        ctx.arc(1, 0, 1, Math.PI / 2, Math.PI);
    }
    paintEndEnd(ctx) {
        ctx.arc(0, 0, 1, 0, Math.PI / 2);
    }
}

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
/**
 * Paints squircle corners.
 */
class SquircleCornerShapePainter {
    constructor() {
        this.bleed = 1.8;
    }
    paintStartStart(ctx) {
        ctx.translate(0.5, 0.5);
        ctx.rotate(Math.PI);
        ctx.translate(-0.5, -0.5);
        this.paintEndEnd(ctx);
    }
    paintStartEnd(ctx) {
        ctx.translate(0.5, 0.5);
        ctx.rotate(-Math.PI / 2);
        ctx.translate(-0.5, -0.5);
        this.paintEndEnd(ctx);
    }
    paintEndStart(ctx) {
        ctx.translate(0.5, 0.5);
        ctx.rotate(Math.PI / 2);
        ctx.translate(-0.5, -0.5);
        this.paintEndEnd(ctx);
    }
    paintEndEnd(ctx) {
        for (var i = 1; i < 19; i++) {
            ctx.bezierCurveTo(SquircleCornerShapePainter.END_END_POINTS[i - 1].x + SquircleCornerShapePainter.END_END_POINTS[i - 1].dx, SquircleCornerShapePainter.END_END_POINTS[i - 1].y + SquircleCornerShapePainter.END_END_POINTS[i - 1].dy, SquircleCornerShapePainter.END_END_POINTS[i].x - SquircleCornerShapePainter.END_END_POINTS[i].dx, SquircleCornerShapePainter.END_END_POINTS[i].y - SquircleCornerShapePainter.END_END_POINTS[i].dy, SquircleCornerShapePainter.END_END_POINTS[i].x, SquircleCornerShapePainter.END_END_POINTS[i].y);
        }
    }
    /**
     * Builds the END_END_POINTS array, containing points for a bottom right corner. Other corners require a mathematical transformation.
     * The points are at the intersections of (i) the squircle equation x^4 + y^4 = 1 and (ii) a set of radial lines with angles (expressed in radians)
     * set 10 degrees apart from 0 to 90 degrees. Bezier curve points dx and dy are then added for optimum curve smoothing.
     */
    static initialize() {
        const f = 0.2;
        let tan = Math.tan(Math.PI * -1 / 36);
        let ntan = Math.tan(0);
        for (let i = 0; i <= 18; i++) {
            let ptan = tan;
            tan = ntan;
            ntan = Math.tan(Math.PI * (i + 1) / 36);
            const px = Math.pow(1 / (1 + Math.pow(ptan, 4)), 0.25);
            const py = Math.pow(1 - Math.pow(px, 4), 0.25);
            const x = Math.pow(1 / (1 + Math.pow(tan, 4)), 0.25);
            const y = Math.pow(1 - Math.pow(x, 4), 0.25);
            const nx = Math.pow(1 / (1 + Math.pow(ntan, 4)), 0.25);
            const ny = Math.pow(1 - Math.pow(nx, 4), 0.25);
            const dx = (nx - px) * f;
            const dy = (ny - py) * f;
            let point = { x: x, y: y, dx: dx, dy: dy };
            if (i === 0) {
                point = { x: 1, y: 0, dx: 0, dy: 0.035 };
            }
            else if (i === 18) {
                point = { x: 0, y: 1, dx: -0.035, dy: 0 };
            }
            SquircleCornerShapePainter.END_END_POINTS.push(point);
        }
    }
}
SquircleCornerShapePainter.END_END_POINTS = [];
SquircleCornerShapePainter.initialize();

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
/**
 * Enumerates the syntax of a property.
 */
var PropertySyntax;
(function (PropertySyntax) {
    PropertySyntax["Length"] = "length";
    PropertySyntax["Number"] = "number";
})(PropertySyntax || (PropertySyntax = {}));
/**
 * Represents a property used by the corner treatment system.
 */
class Property {
    constructor(name, syntax, inherits, initialValue) {
        this.name = name;
        this.syntax = syntax;
        this.inherits = inherits;
        this.initialValue = initialValue;
    }
}
/**
 * Defines custom css properties used by the corner treatment system.
 */
class CustomCSSProperties {
}
// go/keep-sorted start
CustomCSSProperties.CORNER_TREATMENT = '--_ct-treatment';
CustomCSSProperties.CORNER_TREATMENT_END_END = '--_ct-treatment-end-end';
CustomCSSProperties.CORNER_TREATMENT_END_START = '--_ct-treatment-end-start';
CustomCSSProperties.CORNER_TREATMENT_START_END = '--_ct-treatment-start-end';
CustomCSSProperties.CORNER_TREATMENT_START_START = '--_ct-treatment-start-start';
CustomCSSProperties.DIRECTION = 'direction';
CustomCSSProperties.FILL_COLOR = '--_ct-fill-color';
CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT = '--_ct-focus-ring-treatment';
CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_END_END = '--_ct-focus-ring-treatment-end-end';
CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_END_START = '--_ct-focus-ring-treatment-end-start';
CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_START_END = '--_ct-focus-ring-treatment-start-end';
CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_START_START = '--_ct-focus-ring-treatment-start-start';
CustomCSSProperties.INSET = '--_ct-inset';
CustomCSSProperties.RADIUS_END_END = '--_ct-shape-end-end';
CustomCSSProperties.RADIUS_END_START = '--_ct-shape-end-start';
CustomCSSProperties.RADIUS_START_END = '--_ct-shape-start-end';
CustomCSSProperties.RADIUS_START_START = '--_ct-shape-start-start';
// go/keep-sorted end
// go/keep-sorted start
CustomCSSProperties.BORDER_WIDTH = '--_ct-border-width';
// go/keep-sorted end
// go/keep-sorted start
CustomCSSProperties.ELEVATION_LEVEL = '--_ct-elevation-level';
CustomCSSProperties.ELEVATION_OPACITY = '--_ct-elevation-opacity';
CustomCSSProperties.ELEVATION_PSEUDO_ELEMENT = '--_ct-elevation-pseudo-element';
// go/keep-sorted end
// go/keep-sorted start
CustomCSSProperties.OUTLINE_MAX_WIDTH = '--_ct-outline-max-width';
CustomCSSProperties.OUTLINE_OUTWARD_OFFSET = '--_ct-outline-outward-offset';
CustomCSSProperties.OUTLINE_WIDTH = '--_ct-outline-width';
// go/keep-sorted end
/**
 * Custom properties that need to be registered to facilitate animation and transitions.
 */
CustomCSSProperties.registeredProperties = [
    new Property(CustomCSSProperties.RADIUS_END_END, PropertySyntax.Length, false, '0px'),
    new Property(CustomCSSProperties.RADIUS_END_START, PropertySyntax.Length, false, '0px'),
    new Property(CustomCSSProperties.RADIUS_START_END, PropertySyntax.Length, false, '0px'),
    new Property(CustomCSSProperties.RADIUS_START_START, PropertySyntax.Length, false, '0px'),
    new Property(CustomCSSProperties.BORDER_WIDTH, PropertySyntax.Length, false, '0px'),
    new Property(CustomCSSProperties.ELEVATION_LEVEL, PropertySyntax.Number, false, 1),
    new Property(CustomCSSProperties.ELEVATION_OPACITY, PropertySyntax.Number, false, 1),
    new Property(CustomCSSProperties.OUTLINE_WIDTH, PropertySyntax.Length, false, '0px'),
    new Property(CustomCSSProperties.OUTLINE_MAX_WIDTH, PropertySyntax.Length, false, '0px'),
    new Property(CustomCSSProperties.OUTLINE_OUTWARD_OFFSET, PropertySyntax.Length, false, '0px'),
];

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
var _a$1;
/**
 * A class that registers custom shape painters for Materia web component corner treatments.
 */
class CustomShapePainters {
    static generateRandomString(length) {
        let result = '';
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        const charactersLength = characters.length;
        let counter = 0;
        while (counter < length) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
            counter += 1;
        }
        return result;
    }
    /**
     * Registers a custom shape painter. The supplied name is the value that will be used in the `--mx-corner-treatment` CSS propert
     * and its counterparts for each corner. Logs a warning to the console if a painter with the same name has already been registered.
     * @param name The name of the custom shape painter which must be unique.
     * @param painter The custom painter
     * @returns True if the painter was successfully registered, false if a painter with the same name has already been registered.
     */
    static registerCornerShapePainter(name, painter) {
        if (!this.cornerShapePainters[name]) {
            this.cornerShapePainters[name] = painter;
            return true;
        }
        console.warn(`Materia: A CustomShapePainter called '${name}' has already been registered can cannot be re-registered.`);
        return false;
    }
    /**
     * Get a CornerShapePainter by name. If the painter is not found, a new RoundedCornerShapePainter is returned.
     * @param name
     * @returns a tuple containing a boolean indicating whether the painter was found and the painter itself.
     */
    static getCornerShapePainter(name) {
        if (!this.cornerShapePainters[name]) {
            return [false, new RoundedCornerShapePainter()];
        }
        return [true, this.cornerShapePainters[name]];
    }
}
_a$1 = CustomShapePainters;
CustomShapePainters.id = _a$1.generateRandomString(8);
CustomShapePainters.cornerShapePainters = {};

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
/**
 * Defines built in treatment styles for corners.
 */
var CornerTreatment;
(function (CornerTreatment) {
    /**
     * Regular rounded corner.
     */
    CornerTreatment["ROUNDED"] = "rounded";
    /**
     * Squircle corner using the equation x^4 + y^4 = 1.
     */
    CornerTreatment["SQUIRCLE"] = "squircle";
    /**
     * Cut or bevelled 45 degree corner.
     */
    CornerTreatment["CUT"] = "cut";
})(CornerTreatment || (CornerTreatment = {}));
const CORNER_TREATMENT_PAINTERS = {
    [CornerTreatment.ROUNDED]: new RoundedCornerShapePainter(),
    [CornerTreatment.SQUIRCLE]: new SquircleCornerShapePainter(),
    [CornerTreatment.CUT]: new CutCornerShapePainter(),
};
/**
 * Represents a painter for corner treatments.
 */
class CornerTreatmentPainter {
    /**
     * Returns the standard input properties for all corner treatment painter worklets.
     */
    static get standardInputProperties() {
        return [
            // go/keep-sorted start
            CustomCSSProperties.CORNER_TREATMENT,
            CustomCSSProperties.CORNER_TREATMENT_END_END,
            CustomCSSProperties.CORNER_TREATMENT_END_START,
            CustomCSSProperties.CORNER_TREATMENT_START_END,
            CustomCSSProperties.CORNER_TREATMENT_START_START,
            CustomCSSProperties.DIRECTION,
            CustomCSSProperties.FILL_COLOR,
            CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT,
            CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_END_END,
            CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_END_START,
            CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_START_END,
            CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_START_START,
            CustomCSSProperties.INSET,
            CustomCSSProperties.RADIUS_END_END,
            CustomCSSProperties.RADIUS_END_START,
            CustomCSSProperties.RADIUS_START_END,
            CustomCSSProperties.RADIUS_START_START,
            // go/keep-sorted end
        ];
    }
    /**
     * Gets corner shape painter specifications from paint worklet properties, defaulting to rounded.
     *
     * @param props paint worklet properties
     * @returns the corner treatment specifications
     */
    static parseCornerShapePainterSpecifications(props) {
        return this.buildCornerShapePainterSpecifications(props, CustomCSSProperties.CORNER_TREATMENT, CustomCSSProperties.CORNER_TREATMENT_END_END, CustomCSSProperties.CORNER_TREATMENT_END_START, CustomCSSProperties.CORNER_TREATMENT_START_END, CustomCSSProperties.CORNER_TREATMENT_START_START);
    }
    /**
     * Gets focus ring corner shape painter specifications from paint worklet properties, defaulting to rounded.
     *
     * @param props paint worklet properties
     * @returns the corner treatment specifications
     */
    static parseFocusRingCornerShapePainterSpecifications(props) {
        return this.buildCornerShapePainterSpecifications(props, CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT, CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_END_END, CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_END_START, CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_START_END, CustomCSSProperties.FOCUS_RING_CORNER_TREATMENT_START_START);
    }
    static buildCornerShapePainterSpecifications(props, mainVar, endEndVar, endStartVar, startEndVar, startStartVar) {
        const main = this.getCornerShapePainter(props, mainVar, CORNER_TREATMENT_PAINTERS[CornerTreatment.ROUNDED]);
        return {
            endEnd: this.getCornerShapePainter(props, endEndVar, main),
            endStart: this.getCornerShapePainter(props, endStartVar, main),
            startEnd: this.getCornerShapePainter(props, startEndVar, main),
            startStart: this.getCornerShapePainter(props, startStartVar, main),
        };
    }
    static getCornerShapePainter(props, ctVariable, fallback) {
        const prop = props.get(ctVariable)?.toString() || '';
        if (prop == '') {
            return fallback;
        }
        const cornerTreatment = prop;
        if (Object.values(CornerTreatment).includes(cornerTreatment)) {
            return CORNER_TREATMENT_PAINTERS[cornerTreatment];
        }
        const [hasCustomPainter, customPainter] = CustomShapePainters.getCornerShapePainter(cornerTreatment);
        if (hasCustomPainter) {
            return customPainter;
        }
        console.warn(`Invalid corner treatment: '${cornerTreatment}', using '${fallback}' instead.`);
        return fallback;
    }
    /**
     * Gets the fill color.
     *
     * @param props paint worklet properties
     * @returns the fill color
     */
    static parseFillColor(props) {
        return props.get(CustomCSSProperties.FILL_COLOR) || 'transparent';
    }
    /**
     * Gets the corner radius specification.
     *
     * @param props paint worklet properties
     * @returns the corner radius specification
     */
    static parseCornerRadiusSpecifications(props) {
        return {
            endEnd: parseFloat(props.get(CustomCSSProperties.RADIUS_END_END)) || 0,
            endStart: parseFloat(props.get(CustomCSSProperties.RADIUS_END_START)) || 0,
            startEnd: parseFloat(props.get(CustomCSSProperties.RADIUS_START_END)) || 0,
            startStart: parseFloat(props.get(CustomCSSProperties.RADIUS_START_START)) || 0,
        };
    }
    /**
     * Gets the inset distance.
     *
     * @param props paint worklet properties
     * @returns the inset distance
     */
    static parseInset(props) {
        return parseFloat(props.get(CustomCSSProperties.INSET)) || 0;
    }
    /**
     * Gets the direction from paint worklet properties, defaulting to ltr.
     *
     * @param props paint worklet properties
     * @returns a boolean indicating whether the direction is rtl
     */
    static parseIsRtl(props) {
        return (props.get(CustomCSSProperties.DIRECTION)?.toString().toLowerCase() === "rtl" ?? false);
    }
    /**
     * Paints the shape contained by the geometry, but without adding stroke, fill, or clip.
     * @param ctx the canvas rendering context
     * @param geom the geometry of the element
     * @param p the input properties
     */
    static paintShape(ctx, geom, p) {
        const width = geom.width - 2 * p.inset;
        const height = geom.height - 2 * p.inset;
        const maxRadius = Math.min(width, height) / 2;
        const endEnd = Math.min(maxRadius, p.radiusSpecs.endEnd * p.ctSpecs.endEnd.bleed);
        const endStart = Math.min(maxRadius, p.radiusSpecs.endStart * p.ctSpecs.endStart.bleed);
        const startEnd = Math.min(maxRadius, p.radiusSpecs.startEnd * p.ctSpecs.startEnd.bleed);
        const startStart = Math.min(maxRadius, p.radiusSpecs.startStart * p.ctSpecs.startStart.bleed);
        ctx.save();
        if (p.isRtl) {
            ctx.translate(geom.width, 0);
            ctx.scale(-1, 1);
        }
        ctx.translate(p.inset + p.xOffset, p.inset + p.yOffset);
        ctx.beginPath();
        // Start to the right of the top left corner and move right - painting is done in a clockwise direction assuming left-to-right and top-to-bottom.
        ctx.moveTo(startStart, 0);
        ctx.lineTo(width - startEnd, 0);
        // Top right corner
        if (startEnd > 0) {
            ctx.save();
            ctx.translate(width - startEnd, 0);
            ctx.scale(startEnd, startEnd);
            p.ctSpecs.startEnd.paintStartEnd(ctx);
            ctx.restore();
        }
        // Move down
        ctx.lineTo(width, height - endEnd);
        // Bottom right corner
        if (endEnd > 0) {
            ctx.save();
            ctx.translate(width - endEnd, height - endEnd);
            ctx.scale(endEnd, endEnd);
            p.ctSpecs.endEnd.paintEndEnd(ctx);
            ctx.restore();
        }
        // Move left
        ctx.lineTo(endStart, height);
        // Bottom left corner
        if (endStart > 0) {
            ctx.save();
            ctx.translate(0, height - endStart);
            ctx.scale(endStart, endStart);
            p.ctSpecs.endStart.paintEndStart(ctx);
            ctx.restore();
        }
        // Move up
        ctx.lineTo(0, startStart);
        // Top left corner
        if (startStart > 0) {
            ctx.save();
            ctx.scale(startStart, startStart);
            p.ctSpecs.startStart.paintStartStart(ctx);
            ctx.restore();
        }
        ctx.closePath();
        ctx.restore();
    }
}

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
class CTMaskPainter {
    /**
     * The input properties for the corner treatment painter.
     */
    static get inputProperties() {
        return CornerTreatmentPainter.standardInputProperties;
    }
    /**
     * Clips the painted shape and fills with the prevailing background color. Intended to
     * be used thus: `mask-image: paint(ct-mask-clip-background);`.
     * @param ctx the canvas rendering context
     * @param geom the geometry of the element
     * @param props the input properties
     */
    paintMask(ctx, geom, props) {
        const p = {
            // go/keep-sorted start
            ctSpecs: CornerTreatmentPainter.parseCornerShapePainterSpecifications(props),
            inset: 0,
            isRtl: CornerTreatmentPainter.parseIsRtl(props),
            radiusSpecs: CornerTreatmentPainter.parseCornerRadiusSpecifications(props),
            xOffset: 0,
            yOffset: 0,
            // go/keep-sorted end
        };
        CornerTreatmentPainter.paintShape(ctx, geom, p);
        ctx.clip();
        ctx.fill();
    }
}

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
class CTOutlinePainter {
    /**
     * Clips an outline outside the painted shape and fills with the prevailing background color. Intended to
     * be used thus: `mask-image: paint(ct-outline);`.
     * @param ctx the canvas rendering context
     * @param geom the geometry of the element
     * @param props the input properties
     * @param cspSpecs the corner treatment to apply
     */
    paintOutline(ctx, geom, props, cspSpecs) {
        const width = parseFloat(props.get(CustomCSSProperties.OUTLINE_WIDTH)) || 0;
        if (width <= 0) {
            return;
        }
        const maxWidth = parseFloat(props.get(CustomCSSProperties.OUTLINE_MAX_WIDTH)) || 0;
        const outwardOffset = parseFloat(props.get(CustomCSSProperties.OUTLINE_OUTWARD_OFFSET)) || 0;
        // Set the path to fit the element exactly
        const p = {
            // go/keep-sorted start
            ctSpecs: cspSpecs,
            inset: outwardOffset + maxWidth,
            isRtl: CornerTreatmentPainter.parseIsRtl(props),
            radiusSpecs: CornerTreatmentPainter.parseCornerRadiusSpecifications(props),
            xOffset: 0,
            yOffset: 0,
            // go/keep-sorted end
        };
        ctx.strokeStyle = CornerTreatmentPainter.parseFillColor(props);
        ctx.lineWidth = 2 * (outwardOffset + width);
        CornerTreatmentPainter.paintShape(ctx, geom, p);
        ctx.stroke();
        // Now clip the border inside the outward offset
        ctx.lineWidth = 2 * outwardOffset;
        ctx.globalCompositeOperation = 'destination-out';
        CornerTreatmentPainter.paintShape(ctx, geom, p);
        ctx.stroke();
        ctx.fill();
    }
}

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 */
/**
 * Registers the mx-pw-ct-background paint worklet.
 */
function registerPwCtBackground(registeredClass) {
    // @ts-expect-error
    registerPaint('mx-pw-ct-background', registeredClass);
}
/**
 * Registers the mx-pw-ct-border paint worklet.
 */
function registerPwCtBorder(registeredClass) {
    // @ts-expect-error
    registerPaint('mx-pw-ct-border', registeredClass);
}
/**
 * Registers the mx-pw-ct-elevation-shadow paint worklet.
 */
function registerPwCtElevationShadow(registeredClass) {
    // @ts-expect-error
    registerPaint('mx-pw-ct-elevation-shadow', registeredClass);
}
/**
 * Registers the mx-pw-ct-focus-ring-outline paint worklet.
 */
function registerPwCtFocusRingOutline(registeredClass) {
    // @ts-expect-error
    registerPaint('mx-pw-ct-focus-ring-outline', registeredClass);
}
/**
 * Registers the mx-pw-ct-mask-clip-background paint worklet.
 */
function registerPwCtMaskClipBackground(registeredClass) {
    // @ts-expect-error
    registerPaint('mx-pw-ct-mask-clip-background', registeredClass);
}
/**
 * Registers the mx-pw-ct-outline paint worklet.
 */
function registerPwCtOutline(registeredClass) {
    // @ts-expect-error
    registerPaint('mx-pw-ct-outline', registeredClass);
}

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
registerPwCtBackground(class extends CTMaskPainter {
    /**
     * Clips the painted shape and fills with the prevailing background color. Intended to
     * be used thus: `mask-image: paint(ct-mask-clip-background);`.
     * @param ctx the canvas rendering context
     * @param geom the geometry of the element
     * @param props the input properties
     */
    paint(ctx, geom, props) {
        ctx.fillStyle = CornerTreatmentPainter.parseFillColor(props);
        this.paintMask(ctx, geom, props);
    }
});

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
registerPwCtBorder(class {
    /**
     * The input properties for the corner treatment painter.
     */
    static get inputProperties() {
        return [
            ...CornerTreatmentPainter.standardInputProperties,
            CustomCSSProperties.BORDER_WIDTH,
        ];
    }
    /**
     * Clips a border inside the painted shape and fills with the prevailing background color. Intended to
     * be used thus: `mask-image: paint(ct-border);`.
     * @param ctx the canvas rendering context
     * @param geom the geometry of the element
     * @param props the input properties
     */
    paint(ctx, geom, props) {
        const width = parseFloat(props.get(CustomCSSProperties.BORDER_WIDTH)) || 0;
        if (width <= 0.05) {
            return;
        }
        // Set the path to fit the element exactly
        const p = {
            // go/keep-sorted start
            ctSpecs: CornerTreatmentPainter.parseCornerShapePainterSpecifications(props),
            inset: 0,
            isRtl: CornerTreatmentPainter.parseIsRtl(props),
            radiusSpecs: CornerTreatmentPainter.parseCornerRadiusSpecifications(props),
            xOffset: 0,
            yOffset: 0,
            // go/keep-sorted end
        };
        ctx.strokeStyle = CornerTreatmentPainter.parseFillColor(props);
        ctx.lineWidth = 2 * width;
        CornerTreatmentPainter.paintShape(ctx, geom, p);
        ctx.stroke();
        // Now clip the border inside the painted shape
        ctx.lineWidth = 0;
        ctx.globalCompositeOperation = 'destination-in';
        CornerTreatmentPainter.paintShape(ctx, geom, p);
        ctx.fill();
    }
});

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
var _a;
/**
 * Specifies which pseudo element is required.
 */
var PseudoElement;
(function (PseudoElement) {
    /**
     * The before pseudo element.
     */
    PseudoElement["BEFORE"] = "before";
    /**
     * The after pseudo element.
     */
    PseudoElement["AFTER"] = "after";
})(PseudoElement || (PseudoElement = {}));
registerPwCtElevationShadow((_a = class ElevationShadow {
        /**
         * The input properties for the corner treatment painter.
         */
        static get inputProperties() {
            return [
                ...CornerTreatmentPainter.standardInputProperties,
                CustomCSSProperties.ELEVATION_LEVEL,
                CustomCSSProperties.ELEVATION_OPACITY,
                CustomCSSProperties.ELEVATION_PSEUDO_ELEMENT,
            ];
        }
        /**
         * Applies a drop shadow to a component, intended to be `md-elevation` and is intended to be used thus: `background: paint(ct-elevation-shadow);`.
         * @param ctx the canvas rendering context
         * @param geom the geometry of the element
         * @param props the input properties
         */
        paint(ctx, geom, props) {
            const p = {
                // go/keep-sorted start
                ctSpecs: CornerTreatmentPainter.parseCornerShapePainterSpecifications(props),
                inset: CornerTreatmentPainter.parseInset(props),
                isRtl: CornerTreatmentPainter.parseIsRtl(props),
                radiusSpecs: CornerTreatmentPainter.parseCornerRadiusSpecifications(props),
                xOffset: 0,
                yOffset: 0,
                // go/keep-sorted end
            };
            const level = Math.max(0, Math.min(5, parseFloat(props.get(CustomCSSProperties.ELEVATION_LEVEL)) || 0));
            // Don't paint if level is zero (or very small), to avoid undesired artifacts.
            if (level < 0.1) {
                return;
            }
            const fillColor = CornerTreatmentPainter.parseFillColor(props);
            const opacity = parseFloat(props.get(CustomCSSProperties.ELEVATION_OPACITY)) || 1;
            const pseudoElement = (props.get(CustomCSSProperties.ELEVATION_PSEUDO_ELEMENT) || PseudoElement.BEFORE);
            let { dsY, dsBlur, spread } = pseudoElement == PseudoElement.BEFORE
                ? this.interpolate(level, ElevationShadow.BEFORE_POINTS)
                : this.interpolate(level, ElevationShadow.AFTER_POINTS);
            // Paint the shadow, with the actual shape hidden off the canvas at x < 0.
            let ap = {
                ...p,
                inset: p.inset - spread,
                xOffset: -2 * geom.width,
            };
            ctx.fillStyle = fillColor;
            ctx.globalAlpha = opacity;
            ctx.shadowOffsetX = 2 * geom.width;
            ctx.shadowOffsetY = dsY;
            ctx.shadowBlur = dsBlur;
            ctx.shadowColor = fillColor;
            CornerTreatmentPainter.paintShape(ctx, geom, ap);
            ctx.fill();
            // Paint a shape in the visible area with a global composite operation to punch out the shadow.
            ap = {
                ...p,
                // Marginally increase the inset because painting isn't perfect, and this eliminates slender gaps.
                inset: p.inset + 0.1,
                xOffset: 0,
            };
            ctx.globalAlpha = 1;
            ctx.globalCompositeOperation = "destination-out";
            CornerTreatmentPainter.paintShape(ctx, geom, ap);
            ctx.fill();
        }
        interpolate(level, dataPoints) {
            // Calculate the indices of the two data points to interpolate between
            const index1 = Math.floor(level);
            const index2 = Math.min(index1 + 1, 5);
            // Calculate the fraction between the two data points
            const fraction = level - index1;
            // Perform the linear interpolation
            const dsY = dataPoints[index1].dsY * (1 - fraction) + dataPoints[index2].dsY * fraction;
            const dsBlur = dataPoints[index1].dsBlur * (1 - fraction) + dataPoints[index2].dsBlur * fraction;
            const spread = dataPoints[index1].spread * (1 - fraction) + dataPoints[index2].spread * fraction;
            return { dsY, dsBlur, spread };
        }
    },
    _a.BEFORE_POINTS = [
        { dsY: 0, dsBlur: 0, spread: 0 },
        { dsY: 1, dsBlur: 2, spread: 0 },
        { dsY: 1, dsBlur: 2, spread: 0 },
        { dsY: 1, dsBlur: 3, spread: 0 },
        { dsY: 2, dsBlur: 3, spread: 0 },
        { dsY: 4, dsBlur: 4, spread: 0 },
    ],
    _a.AFTER_POINTS = [
        { dsY: 0, dsBlur: 0, spread: 0 },
        { dsY: 1, dsBlur: 3, spread: 1 },
        { dsY: 2, dsBlur: 6, spread: 2 },
        { dsY: 4, dsBlur: 8, spread: 3 },
        { dsY: 6, dsBlur: 10, spread: 4 },
        { dsY: 8, dsBlur: 12, spread: 6 },
    ],
    _a));

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
registerPwCtFocusRingOutline(class extends CTOutlinePainter {
    /**
     * The input properties for the corner treatment painter.
     */
    static get inputProperties() {
        return [
            ...CornerTreatmentPainter.standardInputProperties,
            CustomCSSProperties.OUTLINE_MAX_WIDTH,
            CustomCSSProperties.OUTLINE_OUTWARD_OFFSET,
            CustomCSSProperties.OUTLINE_WIDTH,
        ];
    }
    /**
     * Clips an outline outside the painted shape and fills with the prevailing background color. Intended to
     * be used thus: `mask-image: paint(ct-outline);`.
     * @param ctx the canvas rendering context
     * @param geom the geometry of the element
     * @param props the input properties
     */
    paint(ctx, geom, props) {
        this.paintOutline(ctx, geom, props, CornerTreatmentPainter.parseFocusRingCornerShapePainterSpecifications(props));
    }
});

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
registerPwCtMaskClipBackground(class extends CTMaskPainter {
    /**
     * Clips the painted shape and fills with the prevailing background color. Intended to
     * be used thus: `mask-image: paint(ct-mask-clip-background);`.
     * @param ctx the canvas rendering context
     * @param geom the geometry of the element
     * @param props the input properties
     */
    paint(ctx, geom, props) {
        this.paintMask(ctx, geom, props);
    }
});

/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 *
 * SPDX-License-Identifier: xyzzy
 *
 * This work is held in source code control.
 */
registerPwCtOutline(class extends CTOutlinePainter {
    /**
     * The input properties for the corner treatment painter.
     */
    static get inputProperties() {
        return [
            ...CornerTreatmentPainter.standardInputProperties,
            CustomCSSProperties.OUTLINE_MAX_WIDTH,
            CustomCSSProperties.OUTLINE_OUTWARD_OFFSET,
            CustomCSSProperties.OUTLINE_WIDTH,
        ];
    }
    /**
     * Clips an outline outside the painted shape and fills with the prevailing background color. Intended to
     * be used thus: `mask-image: paint(ct-outline);`.
     * @param ctx the canvas rendering context
     * @param geom the geometry of the element
     * @param props the input properties
     */
    paint(ctx, geom, props) {
        this.paintOutline(ctx, geom, props, CornerTreatmentPainter.parseCornerShapePainterSpecifications(props));
    }
});

export { CTMaskPainter, CTOutlinePainter, CornerTreatment, CornerTreatmentPainter, CustomCSSProperties, CustomShapePainters, CutCornerShapePainter, RoundedCornerShapePainter, SquircleCornerShapePainter };
