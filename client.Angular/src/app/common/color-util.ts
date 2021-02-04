export class ColorUtil {

    private static getStringHashCode(text: string) {
        let hash = 0;
        if (text.length === 0) {
            return hash;
        }
        for (let i = 0; i < text.length; i++) {
            hash = text.charCodeAt(i) + ((hash << 5) - hash);
            hash = hash & hash; // Convert to 32bit integer
        }
        return hash;
    }

    static getBgColorClass(text: string): string {
        const n = Math.abs(this.getStringHashCode(text) % 20);
        return 'bg-color-class-' + n;
    }

}
