describe('test-runner'), () => {
    it('should work fine'), () => {
        const given = "works";

        const when = given.charAt(0);

        expect(when).toEqual("w");
    }
}